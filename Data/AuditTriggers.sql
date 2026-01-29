USE [EHRBilling]
GO

/*
==============================================================================
FILE: AuditTriggers.sql
DESCRIPTION: Final Master Security Script for EHR Billing System.
CONTAINS:
1. DML Audit Triggers (Track UPDATE/DELETE for all core tables)
2. Tamper Protection (Block manual changes to AuditLog)
3. DDL Protection (Block table drops/alterations)
==============================================================================
*/

-- =============================================
-- 1. UTILITY: TAMPER PROTECTION FOR AuditLog
-- =============================================
CREATE OR ALTER TRIGGER [Healthcare].[trg_ProtectAuditLog]
ON [Healthcare].[AuditLog]
FOR UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    PRINT 'CRITICAL SECURITY: Manual modification or deletion of Audit history is NOT allowed.';
    ROLLBACK TRANSACTION;
END
GO

-- =============================================
-- 2. DML AUDIT TRIGGERS (Tracking Changes)
-- =============================================

-- Macro pattern for all tables:
-- If UPDATE: log old and new
-- If DELETE: log old and NULL new

-- PATIENTS
CREATE OR ALTER TRIGGER [Healthcare].[trg_AuditPatients] ON [Healthcare].[Patients] AFTER UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Op NVARCHAR(10) = (CASE WHEN EXISTS(SELECT 1 FROM inserted) THEN 'UPDATE' ELSE 'DELETE' END);
    INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Operation, OldValue, NewValue, ChangedBy, ChangedDate)
    SELECT 'Patients', ISNULL(i.PatientId, d.PatientId), @Op, (SELECT * FROM (SELECT d.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), (SELECT * FROM (SELECT i.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), SYSTEM_USER, GETDATE() FROM deleted d LEFT JOIN inserted i ON d.PatientId = i.PatientId;
END
GO

-- BILLS
CREATE OR ALTER TRIGGER [Healthcare].[trg_AuditBills] ON [Healthcare].[Bills] AFTER UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Op NVARCHAR(10) = (CASE WHEN EXISTS(SELECT 1 FROM inserted) THEN 'UPDATE' ELSE 'DELETE' END);
    INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Operation, OldValue, NewValue, ChangedBy, ChangedDate)
    SELECT 'Bills', ISNULL(i.BillId, d.BillId), @Op, (SELECT * FROM (SELECT d.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), (SELECT * FROM (SELECT i.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), SYSTEM_USER, GETDATE() FROM deleted d LEFT JOIN inserted i ON d.BillId = i.BillId;
END
GO

-- ADMISSIONS
CREATE OR ALTER TRIGGER [Healthcare].[trg_AuditAdmissions] ON [Healthcare].[Admissions] AFTER UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Op NVARCHAR(10) = (CASE WHEN EXISTS(SELECT 1 FROM inserted) THEN 'UPDATE' ELSE 'DELETE' END);
    INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Operation, OldValue, NewValue, ChangedBy, ChangedDate)
    SELECT 'Admissions', ISNULL(i.AdmissionId, d.AdmissionId), @Op, (SELECT * FROM (SELECT d.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), (SELECT * FROM (SELECT i.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), SYSTEM_USER, GETDATE() FROM deleted d LEFT JOIN inserted i ON d.AdmissionId = i.AdmissionId;
END
GO

-- PAYMENTS
CREATE OR ALTER TRIGGER [Healthcare].[trg_AuditPayments] ON [Healthcare].[Payments] AFTER UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Op NVARCHAR(10) = (CASE WHEN EXISTS(SELECT 1 FROM inserted) THEN 'UPDATE' ELSE 'DELETE' END);
    INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Operation, OldValue, NewValue, ChangedBy, ChangedDate)
    SELECT 'Payments', ISNULL(i.PaymentId, d.PaymentId), @Op, (SELECT * FROM (SELECT d.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), (SELECT * FROM (SELECT i.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), SYSTEM_USER, GETDATE() FROM deleted d LEFT JOIN inserted i ON d.PaymentId = i.PaymentId;
END
GO

-- APPOINTMENTS
CREATE OR ALTER TRIGGER [Healthcare].[trg_AuditAppointment] ON [Healthcare].[Appointment] AFTER UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Op NVARCHAR(10) = (CASE WHEN EXISTS(SELECT 1 FROM inserted) THEN 'UPDATE' ELSE 'DELETE' END);
    INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Operation, OldValue, NewValue, ChangedBy, ChangedDate)
    SELECT 'Appointment', ISNULL(i.AppointmentId, d.AppointmentId), @Op, (SELECT * FROM (SELECT d.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), (SELECT * FROM (SELECT i.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), SYSTEM_USER, GETDATE() FROM deleted d LEFT JOIN inserted i ON d.AppointmentId = i.AppointmentId;
END
GO

-- LAB ORDERS
CREATE OR ALTER TRIGGER [Healthcare].[trg_AuditLabOrder] ON [Healthcare].[LabOrder] AFTER UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Op NVARCHAR(10) = (CASE WHEN EXISTS(SELECT 1 FROM inserted) THEN 'UPDATE' ELSE 'DELETE' END);
    INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Operation, OldValue, NewValue, ChangedBy, ChangedDate)
    SELECT 'LabOrder', ISNULL(i.LabOrderId, d.LabOrderId), @Op, (SELECT * FROM (SELECT d.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), (SELECT * FROM (SELECT i.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), SYSTEM_USER, GETDATE() FROM deleted d LEFT JOIN inserted i ON d.LabOrderId = i.LabOrderId;
END
GO

-- MEDICINES
CREATE OR ALTER TRIGGER [Healthcare].[trg_AuditMedicines] ON [Healthcare].[Medicines] AFTER UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Op NVARCHAR(10) = (CASE WHEN EXISTS(SELECT 1 FROM inserted) THEN 'UPDATE' ELSE 'DELETE' END);
    INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Operation, OldValue, NewValue, ChangedBy, ChangedDate)
    SELECT 'Medicines', ISNULL(i.MedicineId, d.MedicineId), @Op, (SELECT * FROM (SELECT d.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), (SELECT * FROM (SELECT i.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), SYSTEM_USER, GETDATE() FROM deleted d LEFT JOIN inserted i ON d.MedicineId = i.MedicineId;
END
GO

-- PRESCRIPTIONS
CREATE OR ALTER TRIGGER [Healthcare].[trg_AuditPrescriptions] ON [Healthcare].[Prescriptions] AFTER UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Op NVARCHAR(10) = (CASE WHEN EXISTS(SELECT 1 FROM inserted) THEN 'UPDATE' ELSE 'DELETE' END);
    INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Operation, OldValue, NewValue, ChangedBy, ChangedDate)
    SELECT 'Prescriptions', ISNULL(i.PrescriptionId, d.PrescriptionId), @Op, (SELECT * FROM (SELECT d.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), (SELECT * FROM (SELECT i.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), SYSTEM_USER, GETDATE() FROM deleted d LEFT JOIN inserted i ON d.PrescriptionId = i.PrescriptionId;
END
GO

-- DOCTORS
CREATE OR ALTER TRIGGER [Healthcare].[trg_AuditDoctor] ON [Healthcare].[Doctor] AFTER UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Op NVARCHAR(10) = (CASE WHEN EXISTS(SELECT 1 FROM inserted) THEN 'UPDATE' ELSE 'DELETE' END);
    INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Operation, OldValue, NewValue, ChangedBy, ChangedDate)
    SELECT 'Doctor', ISNULL(i.DoctorId, d.DoctorId), @Op, (SELECT * FROM (SELECT d.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), (SELECT * FROM (SELECT i.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), SYSTEM_USER, GETDATE() FROM deleted d LEFT JOIN inserted i ON d.DoctorId = i.DoctorId;
END
GO

-- INSURANCE PROVIDERS
CREATE OR ALTER TRIGGER [Healthcare].[trg_AuditInsuranceProviders] ON [Healthcare].[InsuranceProviders] AFTER UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Op NVARCHAR(10) = (CASE WHEN EXISTS(SELECT 1 FROM inserted) THEN 'UPDATE' ELSE 'DELETE' END);
    INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Operation, OldValue, NewValue, ChangedBy, ChangedDate)
    SELECT 'InsuranceProviders', ISNULL(i.ProviderId, d.ProviderId), @Op, (SELECT * FROM (SELECT d.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), (SELECT * FROM (SELECT i.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), SYSTEM_USER, GETDATE() FROM deleted d LEFT JOIN inserted i ON d.ProviderId = i.ProviderId;
END
GO

-- SERVICES MASTER
CREATE OR ALTER TRIGGER [Healthcare].[trg_AuditServicesMaster] ON [Healthcare].[ServicesMaster] AFTER UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Op NVARCHAR(10) = (CASE WHEN EXISTS(SELECT 1 FROM inserted) THEN 'UPDATE' ELSE 'DELETE' END);
    INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Operation, OldValue, NewValue, ChangedBy, ChangedDate)
    SELECT 'ServicesMaster', ISNULL(i.ServiceId, d.ServiceId), @Op, (SELECT * FROM (SELECT d.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), (SELECT * FROM (SELECT i.*) AS x FOR JSON PATH, WITHOUT_ARRAY_WRAPPER), SYSTEM_USER, GETDATE() FROM deleted d LEFT JOIN inserted i ON d.ServiceId = i.ServiceId;
END
GO

-- =============================================
-- 3. DDL SECURITY: PREVENT DROPPING TABLES
-- =============================================
IF EXISTS (SELECT * FROM sys.triggers WHERE parent_class = 0 AND name = 'trg_PreventTableDrop')
    DROP TRIGGER [trg_PreventTableDrop] ON DATABASE;
GO

CREATE TRIGGER [trg_PreventTableDrop]
ON DATABASE
FOR DROP_TABLE, ALTER_TABLE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Data XML = EVENTDATA();
    DECLARE @ObjName NVARCHAR(255) = @Data.value('(/EVENT_INSTANCE/ObjectName)[1]', 'NVARCHAR(255)');
    DECLARE @SchemaName NVARCHAR(255) = @Data.value('(/EVENT_INSTANCE/SchemaName)[1]', 'NVARCHAR(255)');
    DECLARE @EvType NVARCHAR(100) = @Data.value('(/EVENT_INSTANCE/EventType)[1]', 'NVARCHAR(100)');

    -- Log unauthorized DDL attempt to AuditLog
    INSERT INTO [Healthcare].[AuditLog] (TableName, RecordId, Operation, OldValue, NewValue, ChangedBy, ChangedDate)
    VALUES (
        @ObjName, 
        0, 
        'BLOCKED_' + @EvType, 
        CAST(@Data AS NVARCHAR(MAX)), 
        'SECURITY BLOCK: ACTION REJECTED', 
        SYSTEM_USER, 
        GETDATE()
    );

    IF @SchemaName = 'Healthcare'
    BEGIN
        PRINT 'CRITICAL: Database schema [Healthcare] is protected. Unauthorized DROP/ALTER is BLOCKED.';
        ROLLBACK;
    END
END
GO

ENABLE TRIGGER [trg_PreventTableDrop] ON DATABASE;
GO
