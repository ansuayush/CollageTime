/****** Object:  Table [dbo].[TimeCardSessionInOutConfigs]    Script Date: 2/19/2018 6:35:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TimeCardSessionInOutConfigs](
	[TimeCardSessionId] [int] IDENTITY(1,1) NOT NULL,
	[Toggle] [bit] NULL,
	[MaxIn] [int] NULL,
	[MaxOut] [int] NULL
) ON [PRIMARY]

GO
