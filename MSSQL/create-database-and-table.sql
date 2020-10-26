CREATE DATABASE HospitalBDatabase;
GO

USE HospitalBDatabase;
GO

CREATE TABLE [Appointment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AppointmentId] [varchar](50) NOT NULL,
	[DoctorId] [varchar](max) NOT NULL,
	[Patient] [varchar](max) NULL,
	[StartTime] [int] NOT NULL,
	[EndTime] [int] NULL,
	[RealEndTime] [int] NULL,
	[AppointmentStatus] [varchar](max) NULL,
 CONSTRAINT [PK_Session] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

