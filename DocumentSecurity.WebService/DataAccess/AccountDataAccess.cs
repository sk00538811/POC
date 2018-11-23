using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySetting = DocumentSecurity.WebService.Setting;
using MyUser = DocumentSecurity.WebService.Models.UserModal;
using MyRole = DocumentSecurity.WebService.Models.RoleModal;
namespace DocumentSecurity.WebService.DataAccess
{
    public class AccountDataAccess
    {
        public MyUser GetUserDetailBuId(string userid)
        {
            MyUser o = null;
            DataSet ds = new DataSet("data");
            using (SqlConnection con = new SqlConnection(MySetting.AppDbSetting.DbConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(MySetting.AppSqlQuery.QGetUserDetailById, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserId", userid);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.TableMappings.Add("Table", MyUser.DataTables.dtUser);
                        da.TableMappings.Add("Table1",  MyRole.DataTables.dtRoles);
                        da.Fill(ds);
                    }
                }
            }
            if (ds.Tables.Count >= 2)
            {
                o = (from u in ds.Tables[MyUser.DataTables.dtUser].AsEnumerable()
                     select new MyUser
                     {
                         Id = u.Field<string>(MyUser.Fields.Id),
                         UserName = u.Field<string>(MyUser.Fields.UserName),
                         PasswordHash = u.Field<string>(MyUser.Fields.PasswordHash),
                         Email = u.Field<string>(MyUser.Fields.Email),
                         IsDeleted = u.Field<bool?>(MyUser.Fields.IsDeleted),
                         Roles = (from r in ds.Tables[MyRole.DataTables.dtRoles].AsEnumerable()
                                  select new MyRole
                                  {
                                      UserId = r.Field<string>(MyRole.Fields.UserId),
                                      RoleId = r.Field<string>(MyRole.Fields.RoleId),
                                      RoleName = r.Field<string>(MyRole.Fields.RoleName),
                                  }).ToList()
                     }
                    ).FirstOrDefault();

            }
            return o;
        }
        public MyUser GetUserDetailByUserName(string username)
        {
            MyUser o = null;
            DataSet ds = new DataSet("data");
            using (SqlConnection con = new SqlConnection(MySetting.AppDbSetting.DbConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(MySetting.AppSqlQuery.QGetUserDetailByUserName, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserName", username);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.TableMappings.Add("Table", MyUser.DataTables.dtUser);
                        da.TableMappings.Add("Table1",  MyRole.DataTables.dtRoles);
                        da.Fill(ds);
                    }
                }
            }
            if (ds.Tables.Count >= 2)
            {
                o = (from u in ds.Tables[MyUser.DataTables.dtUser].AsEnumerable()
                     select new MyUser
                     {
                         Id = u.Field<string>(MyUser.Fields.Id),
                         UserName = u.Field<string>(MyUser.Fields.UserName),
                         PasswordHash = u.Field<string>(MyUser.Fields.PasswordHash),
                         Email = u.Field<string>(MyUser.Fields.Email),
                         IsDeleted = u.Field<bool?>(MyUser.Fields.IsDeleted),
                         Roles = (from r in ds.Tables[MyRole.DataTables.dtRoles].AsEnumerable()
                                  select new MyRole
                                  {
                                      UserId = r.Field<string>(MyRole.Fields.UserId),
                                      RoleId = r.Field<string>(MyRole.Fields.RoleId),
                                       RoleName=  r.Field<string>(MyRole.Fields.RoleName),
                                  }).ToList()
                     }
                    ).FirstOrDefault();

            }
            return o;
        }
        public List<MyUser> GetAllUserDetail(string userid)
        {
            List<MyUser> ol = null;
            DataSet ds = new DataSet("data");
            using (SqlConnection con = new SqlConnection(MySetting.AppDbSetting.DbConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(MySetting.AppSqlQuery.QGetAllUserDetail, con))
                {
                    cmd.CommandType = CommandType.Text; 
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.TableMappings.Add("Table", MyUser.DataTables.dtUser);
                        da.TableMappings.Add("Table1",  MyRole.DataTables.dtRoles);
                        da.Fill(ds);
                    }
                }
            }
            if (ds.Tables.Count >= 2)
            {
                ol = (from u in ds.Tables[MyUser.DataTables.dtUser].AsEnumerable()
                     select new MyUser
                     {
                         Id = u.Field<string>(MyUser.Fields.Id),
                         UserName = u.Field<string>(MyUser.Fields.UserName),
                         PasswordHash = u.Field<string>(MyUser.Fields.PasswordHash),
                         Email = u.Field<string>(MyUser.Fields.Email),
                         IsDeleted = u.Field<bool?>(MyUser.Fields.IsDeleted),
                         Roles = (from r in ds.Tables[MyRole.DataTables.dtRoles].AsEnumerable()
                                  where r.Field<string>(MyRole.Fields.UserId) == u.Field<string>(MyUser.Fields.Id)
                                  select new MyRole
                                  {
                                      UserId = r.Field<string>(MyRole.Fields.UserId),
                                      RoleId = r.Field<string>(MyRole.Fields.RoleId),
                                       RoleName = r.Field<string>(MyRole.Fields.RoleName),
                                  }).ToList()
                     }
                    ).ToList();

            }
            return ol;
        }

    }
}