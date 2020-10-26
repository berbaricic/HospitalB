USE [HospitalBDatabase]
GO

CREATE PROCEDURE [dbo].[PersistAppointment_StoredProcedure]
	@AppointmentId varchar(50), 
	@DoctorId varchar(max),
	@Patient varchar(max),
	@StartTime int,
	@EndTime int,
	@RealEndTime int,
	@AppointmentStatus varchar(max)
AS
BEGIN TRY
	BEGIN TRANSACTION
	INSERT INTO Session VALUES (@AppointmentId, @DoctorId, @Patient, @StartTime, @EndTime, @RealEndTime, @AppointmentStatus);
	COMMIT TRANSACTION
END TRY
BEGIN CATCH
	ROLLBACK TRANSACTION
END CATCH