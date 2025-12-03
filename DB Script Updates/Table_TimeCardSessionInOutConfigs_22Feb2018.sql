DROP TABLE [dbo].[TimeCardSessionInOutConfigs]
GO
/****** Object:  Table [dbo].[TimeCardSessionInOutConfigs]    Script Date: 2/23/2018 3:02:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TimeCardSessionInOutConfigs](
	[TimeCardSessionId] [int] IDENTITY(1,1) NOT NULL,
	[Toggle] [bit] NULL,
	[Session] [varchar](50) NULL,
	[MaxHours] [int] NULL
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[TimeCardSessionInOutConfigs] ON 

GO
INSERT [dbo].[TimeCardSessionInOutConfigs] ([TimeCardSessionId], [Toggle], [Session], [MaxHours]) VALUES (1, 1, N'In', 3)
GO
INSERT [dbo].[TimeCardSessionInOutConfigs] ([TimeCardSessionId], [Toggle], [Session], [MaxHours]) VALUES (2, 0, N'Out', 30)
GO
SET IDENTITY_INSERT [dbo].[TimeCardSessionInOutConfigs] OFF
GO
