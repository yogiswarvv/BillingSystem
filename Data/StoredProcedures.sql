USE [EHRBilling]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Schema Updates & Prerequisites
-- =============================================

-- Ensure DeletedDate and DeletedBy columns exist in Patients for Soft Delete
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Healthcare].[Patients]') AND name = 'DeletedDate')
BEGIN
    ALTER TABLE [Healthcare].[Patients] ADD [DeletedDate] DATETIME NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Healthcare].[Patients]') AND name = 'DeletedBy')
BEGIN
    ALTER TABLE [Healthcare].[Patients] ADD [DeletedBy] NVARCHAR(100) NULL;
END
GO

-- Ensure Cost column exists in LabOrder for "Store cost" requirement
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Healthcare].[LabOrder]') AND name = 'Cost')
BEGIN
    ALTER TABLE [Healthcare].[LabOrder] ADD [Cost] DECIMAL(18,2) DEFAULT 0;
END
GO

-- Create Type for Table-Valued Parameter (Multi Lab Order)
IF TYPE_ID(N'[Healthcare].[LabTestOrderType]') IS NULL
BEGIN
    CREATE TYPE [Healthcare].[LabTestOrderType] AS TABLE(
        [TestName] NVARCHAR(200),
        [Cost] DECIMAL(18,2)
    );
END
GO

-- =============================================
-- 1. Patient Registration
-- =============================================
CREATE OR ALTER PROCEDURE [Healthcare].[usp_RegisterPatient]
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @DateOfBirth DATETIME,
    @Gender INT,
    @MobileNumber NVARCHAR(10),
    @Email NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validation
        IF EXISTS (SELECT 1 FROM [Healthcare].[Patients] WHERE [MobileNumber] = @MobileNumber)
        BEGIN
            THROW 50001, 'Patient with this mobile number already exists.', 1;
        END

        -- Insert
        INSERT INTO [Healthcare].[Patients]
        (
            [FirstName], [LastName], [DateOfBirth], [Gender], 
            [MobileNumber], [Email], [IsActive], [CreatedDate]
        )
        VALUES
        (
            @FirstName, @LastName, @DateOfBirth, @Gender,
            @MobileNumber, @Email, 1, GETDATE()
        );

        -- Return the new PatientId
        SELECT SCOPE_IDENTITY() AS PatientId;
        
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- =============================================
-- 2. Patient Visit Log (Logs a visit via Appointment table)
-- =============================================
CREATE OR ALTER PROCEDURE [Healthcare].[usp_LogPatientVisit]
    @PatientId INT,
    @DoctorId INT,
    @VisitDate DATETIME,
    @Reason NVARCHAR(500),
    @Status NVARCHAR(20) = 'Completed' -- Default to Completed for a "Log"
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validate Foreign Keys
        IF NOT EXISTS (SELECT 1 FROM [Healthcare].[Patients] WHERE [PatientId] = @PatientId)
            THROW 50002, 'Invalid Patient ID.', 1;
            
        IF @DoctorId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Healthcare].[Doctor] WHERE [DoctorId] = @DoctorId)
            THROW 50003, 'Invalid Doctor ID.', 1;

        -- Determine Doctor Name if ID is provided
        DECLARE @DoctorName NVARCHAR(100) = '';
        IF @DoctorId IS NOT NULL
        BEGIN
             SELECT @DoctorName = [FirstName] + ' ' + [LastName] 
             FROM [Healthcare].[Doctor] 
             WHERE [DoctorId] = @DoctorId;
        END

        -- Insert Appointment meant as a Visit Log
        INSERT INTO [Healthcare].[Appointment]
        (
            [PatientId], [DoctorId], [DoctorName], [AppointmentDate], 
            [AppointmentTime], [Reason], [Status], [CreatedDate]
        )
        VALUES
        (
            @PatientId, @DoctorId, @DoctorName, CAST(@VisitDate AS DATE),
            CAST(@VisitDate AS TIME), @Reason, @Status, GETDATE()
        );

        SELECT SCOPE_IDENTITY() AS VisitId;
        
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- =============================================
-- 3. Soft Delete Patient
-- =============================================
CREATE OR ALTER PROCEDURE [Healthcare].[usp_SoftDeletePatient]
    @PatientId INT,
    @DeletedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM [Healthcare].[Patients] WHERE [PatientId] = @PatientId)
            THROW 50004, 'Patient not found.', 1;

        UPDATE [Healthcare].[Patients]
        SET 
            [IsActive] = 0,
            [DeletedDate] = GETDATE(),
            [DeletedBy] = @DeletedBy
        WHERE [PatientId] = @PatientId;

        PRINT 'Patient deactivated successfully.';

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- =============================================
-- 4. Laboratory Test Order (Multiple)
-- Support ordering multiple lab tests in a single request (Table-Valued Parameter).
-- =============================================
CREATE OR ALTER PROCEDURE [Healthcare].[usp_OrderLabTests]
    @AppointmentId INT,
    @TestOrders [Healthcare].[LabTestOrderType] READONLY
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Validate Appointment
        IF NOT EXISTS (SELECT 1 FROM [Healthcare].[Appointment] WHERE [AppointmentId] = @AppointmentId)
            THROW 50005, 'Invalid Appointment ID.', 1;

        -- Insert Lab Orders from TVP
        INSERT INTO [Healthcare].[LabOrder]
        (
            [AppointmentId], [TestName], [Cost], [OrderDate], [Status], [IsPaid]
        )
        SELECT 
            @AppointmentId,
            t.TestName,
            t.Cost,
            GETDATE(),
            'Pending',
            0
        FROM @TestOrders t;

        COMMIT TRANSACTION;
        PRINT 'Lab tests ordered successfully.';
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- =============================================
-- 5. Room Admission
-- =============================================
CREATE OR ALTER PROCEDURE [Healthcare].[usp_AdmitPatient]
    @PatientId INT,
    @AdmitDate DATETIME,
    @FeePerDay DECIMAL(18,2) = 2000.00
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validate Patient
        IF NOT EXISTS (SELECT 1 FROM [Healthcare].[Patients] WHERE [PatientId] = @PatientId)
            THROW 50006, 'Invalid Patient ID.', 1;

        -- Prevent duplicate active admissions (where DischargeDate is NULL)
        -- Or duplicate admission for the *same date* as per requirement
        IF EXISTS (SELECT 1 FROM [Healthcare].[Admissions] 
                   WHERE [PatientId] = @PatientId 
                   AND CAST([AdmitDate] AS DATE) = CAST(@AdmitDate AS DATE))
        BEGIN
            THROW 50007, 'Patient is already admitted on this date.', 1;
        END
        
        IF EXISTS (SELECT 1 FROM [Healthcare].[Admissions] WHERE [PatientId] = @PatientId AND [DischargeDate] IS NULL)
        BEGIN
             THROW 50008, 'Patient is currently admitted and not discharged.', 1;
        END

        INSERT INTO [Healthcare].[Admissions]
        (
            [PatientId], [AdmitDate], [FeePerDay], [AdmitDays], [IsPaid]
        )
        VALUES
        (
            @PatientId, @AdmitDate, @FeePerDay, 0, 0
        );

        SELECT SCOPE_IDENTITY() AS AdmissionId;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- =============================================
-- 6. Appointment Creation
-- =============================================
CREATE OR ALTER PROCEDURE [Healthcare].[usp_CreateAppointment]
    @PatientId INT,
    @DoctorId INT,
    @AppointmentDate DATETIME,
    @Reason NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @AppDate DATE = CAST(@AppointmentDate AS DATE);
        DECLARE @AppTime TIME = CAST(@AppointmentDate AS TIME);

        -- Validate Patient & Doctor
        IF NOT EXISTS (SELECT 1 FROM [Healthcare].[Patients] WHERE [PatientId] = @PatientId)
            THROW 50009, 'Patient not found.', 1;
            
        IF NOT EXISTS (SELECT 1 FROM [Healthcare].[Doctor] WHERE [DoctorId] = @DoctorId)
            THROW 50010, 'Doctor not found.', 1;

        -- Prevent Overlapping Appointments for the same Doctor
        -- Assumption: Appointment slots are 15 minutes or exact match check
        -- Checking exact match for simplicity as duration isn't in schema, 
        -- Creates a basic conflict check
        IF EXISTS (SELECT 1 FROM [Healthcare].[Appointment] 
                   WHERE [DoctorId] = @DoctorId 
                   AND CAST([AppointmentDate] AS DATE) = @AppDate 
                   AND [AppointmentTime] = @AppTime
                   AND [Status] <> 'Cancelled')
        BEGIN
            THROW 50011, 'Doctor already has an appointment at this time.', 1;
        END

        DECLARE @DoctorName NVARCHAR(100);
        SELECT @DoctorName = [FirstName] + ' ' + [LastName] FROM [Healthcare].[Doctor] WHERE [DoctorId] = @DoctorId;

        INSERT INTO [Healthcare].[Appointment]
        (
            [PatientId], [DoctorId], [DoctorName], [AppointmentDate], 
            [AppointmentTime], [Reason], [Status], [CreatedDate]
        )
        VALUES
        (
            @PatientId, @DoctorId, @DoctorName, @AppDate,
            @AppTime, @Reason, 'Scheduled', GETDATE()
        );

        SELECT SCOPE_IDENTITY() AS AppointmentId;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- =============================================
-- 7. Appointment Test Orders 
-- (Wrapper or specific context for ordering labs for an appointment)
-- =============================================
-- Implementation is technically covered by usp_OrderLabTests (Requirement 4),
-- but we provide an alias if specific logic is needed later.
-- For now, referencing the same logic.
CREATE OR ALTER PROCEDURE [Healthcare].[usp_AddTestsToAppointment]
    @AppointmentId INT,
    @TestOrders [Healthcare].[LabTestOrderType] READONLY
AS
BEGIN
    EXEC [Healthcare].[usp_OrderLabTests] @AppointmentId, @TestOrders;
END
GO

-- =============================================
-- 8. Appointment Status Update
-- =============================================
CREATE OR ALTER PROCEDURE [Healthcare].[usp_UpdateAppointmentStatus]
    @AppointmentId INT,
    @NewStatus NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @CurrentStatus NVARCHAR(20);
        
        SELECT @CurrentStatus = [Status] 
        FROM [Healthcare].[Appointment] 
        WHERE [AppointmentId] = @AppointmentId;

        IF @CurrentStatus IS NULL
            THROW 50012, 'Appointment not found.', 1;

        -- Validate Transitions
        IF @NewStatus NOT IN ('Scheduled', 'Completed', 'Cancelled')
            THROW 50013, 'Invalid status.', 1;

        -- Example Rule: Cannot cancel a completed appointment
        IF @CurrentStatus = 'Completed' AND @NewStatus = 'Cancelled'
            THROW 50014, 'Cannot cancel a completed appointment.', 1;

        -- Example Rule: Cannot complete a cancelled appointment
        IF @CurrentStatus = 'Cancelled' AND @NewStatus = 'Completed'
            THROW 50015, 'Cannot complete a cancelled appointment.', 1;

        UPDATE [Healthcare].[Appointment]
        SET [Status] = @NewStatus
        WHERE [AppointmentId] = @AppointmentId;

        PRINT 'Appointment status updated.';

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- =============================================
-- 9. Search Lab Orders
-- =============================================
CREATE OR ALTER PROCEDURE [Healthcare].[usp_SearchLabOrders]
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        l.LabOrderId, 
        l.TestName, 
        l.Cost, 
        l.OrderDate, 
        l.Status, 
        l.Results, 
        l.IsPaid, 
        l.AppointmentId,
        p.FirstName, 
        p.LastName, 
        p.PatientId,
        l.CompletedDate
    FROM [Healthcare].[LabOrder] l
    JOIN [Healthcare].[Appointment] a ON l.AppointmentId = a.AppointmentId
    JOIN [Healthcare].[Patients] p ON a.PatientId = p.PatientId
    WHERE (@SearchTerm IS NULL OR @SearchTerm = '' 
           OR p.FirstName LIKE '%' + @SearchTerm + '%' 
           OR p.LastName LIKE '%' + @SearchTerm + '%'
           OR (p.FirstName + ' ' + p.LastName) LIKE '%' + @SearchTerm + '%'
           OR CAST(p.PatientId AS NVARCHAR) LIKE '%' + @SearchTerm + '%')
    ORDER BY l.OrderDate DESC;
END
GO

-- =============================================
-- 10. Update Lab Result
-- =============================================
CREATE OR ALTER PROCEDURE [Healthcare].[usp_UpdateLabResult]
    @LabOrderId INT,
    @Status NVARCHAR(20),
    @Results NVARCHAR(1000) = NULL,
    @CompletedDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM [Healthcare].[LabOrder] WHERE [LabOrderId] = @LabOrderId)
            THROW 50017, 'Lab Order not found.', 1;

        UPDATE [Healthcare].[LabOrder]
        SET 
            [Status] = @Status,
            [Results] = @Results,
            [CompletedDate] = @CompletedDate
        WHERE [LabOrderId] = @LabOrderId;

        PRINT 'Lab Order updated successfully.';
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- =============================================
-- 11. Get Patient For Billing (By Id or Mobile)
-- =============================================
CREATE OR ALTER PROCEDURE [Healthcare].[usp_GetPatientForBilling]
    @SearchTerm NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        [PatientId],
        [FirstName],
        [LastName],
        [DateOfBirth],
        [Gender],
        [MobileNumber],
        [Email],
        [Address],
        [IsActive]
    FROM [Healthcare].[Patients]
    WHERE ([MobileNumber] = @SearchTerm OR CAST([PatientId] AS NVARCHAR) = @SearchTerm)
    AND [IsActive] = 1;

END
GO

-- =============================================
-- 12. Check Patient Insurance (By Name & PolicyNo)
-- =============================================
CREATE OR ALTER PROCEDURE [Healthcare].[usp_CheckPatientInsurance]
    @PatientId INT,
    @ProviderName NVARCHAR(100),
    @PolicyNumber NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        i.[InsuranceId],
        i.[PatientId],
        i.[ProviderName],
        i.[PolicyNumber],
        i.[CoveragePercent],
        i.[CoverageType],
        i.[IsActive]
    FROM [Healthcare].[Insurances] i
    INNER JOIN [Healthcare].[InsuranceProviders] p ON i.[ProviderName] = p.[ProviderName]
    WHERE --i.[PatientId] = @PatientId AND 
        i.[ProviderName] = TRIM(@ProviderName)
      AND i.[PolicyNumber] = TRIM(@PolicyNumber)
      AND i.[IsActive] = 1
      AND p.[IsLinked] = 1;

END
GO
