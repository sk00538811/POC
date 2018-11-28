using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace DocumentSecurity.WebService
{
    internal class Setting
    {
        public class AppSetting
        {
            public static readonly string PasswordHash = "P@@Sw0rd";
            public static readonly string SaltKey = "S@LT&KEY";
            public static readonly string VIKey = "@1B2c3D4e5F6g7H8";

        }
        public class AppDbSetting
        {
            public static string DbConnectionString = ConfigurationManager.ConnectionStrings["AppControllerDB"].ConnectionString;
        }
        public static class AppSqlQuery
        {
            /// <summary>
            /// Description: its return single table 1. AspNetRoles  
            /// </summary>
            public static string QGetAllRoles = @"select [RoleId]=[Id],[RoleName]=[Name] from  [dbo].[AspNetRoles]";
            /// <summary>
            /// Description: its return two table 1. AspNetUsers 2. AspNetUserRoles
            /// and take only one parameter @UserID nvarchar(128)
            /// </summary>
            public static string QGetUserDetailById = @"SELECT   [Id] ,[Email]  ,[EmailConfirmed] ,[PasswordHash] ,[SecurityStamp]
                                            ,[PhoneNumber] ,[PhoneNumberConfirmed] ,[TwoFactorEnabled] ,[LockoutEndDateUtc] 
                                            ,[LockoutEnabled] ,[AccessFailedCount] ,[UserName] ,[IsDeleted]
                                              FROM  [dbo].[AspNetUsers]
                                             where [Id]=@UserId; 

                                            SELECT ur.[UserId]  ,ur.[RoleId],  [RoleName]= r.[Name]
                                              FROM [dbo].[AspNetUserRoles] ur inner join [dbo].[AspNetRoles] r
                                                     on ur.[RoleId]=r.[Id]
                                                where [UserId]=@UserId;";
            /// <summary>
            /// Description: its return two table 1. AspNetUsers 2. AspNetUserRoles
            /// and take only one parameter @UserName nvarchar(256)
            /// </summary>
            public static string QGetUserDetailByUserName = @"SELECT   [Id] ,[Email]  ,[EmailConfirmed] ,[PasswordHash] ,[SecurityStamp]
                                            ,[PhoneNumber] ,[PhoneNumberConfirmed] ,[TwoFactorEnabled] ,[LockoutEndDateUtc] 
                                            ,[LockoutEnabled] ,[AccessFailedCount] ,[UserName] ,[IsDeleted]
                                              FROM  [dbo].[AspNetUsers]
                                             where [UserName]=@UserName; 

                                            SELECT ur.[UserId]  ,ur.[RoleId], [RoleName]= r.[Name]
                                              FROM [dbo].[AspNetUserRoles] ur inner join [dbo].[AspNetRoles] r
                                                     on ur.[RoleId]=r.[Id] inner join [dbo].[AspNetUsers] u 
                                                on ur.[UserId]=u.[Id]
                                                where u.[UserName]=@UserName;";
            /// <summary>
            /// Description: its return two table 1. AspNetUsers 2. AspNetUserRoles
            /// </summary>
            public static string QGetAllUserDetail = @"SELECT   [Id] ,[Email]  ,[EmailConfirmed] ,[PasswordHash] ,[SecurityStamp]
                                            ,[PhoneNumber] ,[PhoneNumberConfirmed] ,[TwoFactorEnabled] ,[LockoutEndDateUtc] 
                                            ,[LockoutEnabled] ,[AccessFailedCount] ,[UserName] ,[IsDeleted]
                                              FROM  [dbo].[AspNetUsers] ; 

                                            SELECT ur.[UserId]  ,ur.[RoleId],   [RoleName]= r.[Name]
                                              FROM [dbo].[AspNetUserRoles] ur inner join [dbo].[AspNetRoles] r
                                                     on ur.[RoleId]=r.[Id]  ";
            /// <summary>
            /// Description: its return two true/false
            /// and take only   parameter @Id,@Email,@PasswordHash ,@UserName 
            /// </summary>
            public static string QCreateUser = @"insert into [dbo].[AspNetUsers] ( [Id] ,[Email]  ,[EmailConfirmed] ,[PasswordHash] ,[SecurityStamp]
                                            ,[PhoneNumber] ,[PhoneNumberConfirmed] ,[TwoFactorEnabled] ,[LockoutEndDateUtc] 
                                            ,[LockoutEnabled] ,[AccessFailedCount] ,[UserName] ,[IsDeleted])
                                             values 
                                             ( @Id  ,@Email   ,0 ,@PasswordHash  ,null
                                            ,null ,0 ,0 ,null 
                                            ,0 ,0 ,@UserName  ,0)";
            /// <summary>
            /// Description: its return two true/false
            /// and take only parameter @Id 
            /// </summary>
            public static string QDeleteUser = @"Update [dbo].[AspNetUsers] set [IsDeleted]=1  
                                             where [Id]= @Id  ";
            /// <summary>
            /// Description: its return two true/false
            /// and take only parameter @Id , @PasswordHash
            /// </summary>
            public static string QChangeUserPassword = @"Update [dbo].[AspNetUsers] set [PasswordHash]=@PasswordHash 
                                             where [Id]= @Id  ";
            /// <summary>
            /// Description: its return two true/false
            /// and take only   parameter @UserId  ,@RoleId
            /// </summary>
            public static string QRemoveUserRole = @"delete from [dbo].[AspNetUserRoles] where
                                              [UserId]= @UserId  and [RoleId]=@RoleId ";

            /// <summary>
            /// Description: its return two true/false
            /// and take only   parameter @UserId  ,@RoleId
            /// </summary>
            public static string QAddUserRole = @"Insert into  [dbo].[AspNetUserRoles]( [UserId] ,[RoleId] )
                                             values( @UserId , @RoleId) ";
        }

        public enum GlobalRoles
        {
            SuperAdmin = 1,
            Admin = 2,
            Client = 3
        }
    }
}