USE [2C2PEServiceDB]
GO
SET IDENTITY_INSERT [dbo].[ServiceAccessRight] ON 

INSERT [dbo].[ServiceAccessRight] ([ID], [AuthenticationKey], [PermissionLevel], [Active]) VALUES (1, N'c9a6d776d21054409f4a4b8ec85438ae1710381b0fed7577f321bfb3493082f4', N'CREATE    ', 1)
INSERT [dbo].[ServiceAccessRight] ([ID], [AuthenticationKey], [PermissionLevel], [Active]) VALUES (2, N'c5bfbe7d54b04afb46d11f57acbbc64cacc3daacdd72d3f07273b3bb7953e968', N'GET       ', 1)
SET IDENTITY_INSERT [dbo].[ServiceAccessRight] OFF
GO
