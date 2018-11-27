using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySetting = DocumentSecurity.WebService.Setting;
using MyUser = DocumentSecurity.WebService.Models.UserModal;
using MyUserRole = DocumentSecurity.WebService.Models.UserRoleModal;
using MyRole = DocumentSecurity.WebService.Models.RoleModal;
using Microsoft.AspNet.Identity;

namespace DocumentSecurity.WebService.DataAccess
{
    public class AccountDataAccess
    {
        public MyUser GetUserDetailById(string userid)
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
                        da.TableMappings.Add("Table1", MyUserRole.DataTables.dtUserRoles);
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
                         Roles = (from r in ds.Tables[MyUserRole.DataTables.dtUserRoles].AsEnumerable()
                                  select new MyUserRole
                                  {
                                      UserId = r.Field<string>(MyUserRole.Fields.UserId),
                                      RoleId = r.Field<string>(MyUserRole.Fields.RoleId),
                                      RoleName = r.Field<string>(MyUserRole.Fields.RoleName),
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
                        da.TableMappings.Add("Table1", MyUserRole.DataTables.dtUserRoles);
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
                         Roles = (from r in ds.Tables[MyUserRole.DataTables.dtUserRoles].AsEnumerable()
                                  select new MyUserRole
                                  {
                                      UserId = r.Field<string>(MyUserRole.Fields.UserId),
                                      RoleId = r.Field<string>(MyUserRole.Fields.RoleId),
                                      RoleName = r.Field<string>(MyUserRole.Fields.RoleName),
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
                        da.TableMappings.Add("Table1", MyRole.DataTables.dtRoles);
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
                          Roles = (from r in ds.Tables[MyUserRole.DataTables.dtUserRoles].AsEnumerable()
                                   where r.Field<string>(MyUserRole.Fields.UserId) == u.Field<string>(MyUser.Fields.Id)
                                   select new MyUserRole
                                   {
                                       UserId = r.Field<string>(MyUserRole.Fields.UserId),
                                       RoleId = r.Field<string>(MyUserRole.Fields.RoleId),
                                       RoleName = r.Field<string>(MyUserRole.Fields.RoleName),
                                   }).ToList()
                      }
                    ).ToList();

            }
            return ol;
        }
        public bool   CreateUser(MyUser user)
        {
             

            bool flag = false;
            using (SqlConnection con = new SqlConnection(MySetting.AppDbSetting.DbConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(MySetting.AppSqlQuery.QCreateUser, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Id", user.Id);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@UserName", user.UserName);
                    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    try { con.Open();
                        if (cmd.ExecuteNonQuery() <= 0) {
                            throw new Exception("Unable to create user. Please Contact System Adminstrator.");
                           
                        }else flag = true;
                    }
                    catch (Exception ex)
                    {
                        flag = false;
                        throw ex;
                    }
                    finally { con.Close(); }
                }
            }
            return flag;
        }
        public bool   ChangeUserPassword(MyUser user)
        {
             

            bool flag = false;
            using (SqlConnection con = new SqlConnection(MySetting.AppDbSetting.DbConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(MySetting.AppSqlQuery.QChangeUserPassword, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Id", user.Id); 
                    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    try { con.Open();
                        if (cmd.ExecuteNonQuery() <= 0) {
                            throw new Exception("Unable to change user password. Please Contact System Adminstrator.");
                           
                        }else flag = true;
                    }
                    catch (Exception ex)
                    {
                        flag = false;
                        throw ex;
                    }
                    finally { con.Close(); }
                }
            }
            return flag;
        }
        public bool   DeleteUser(MyUser user)
        {
             

            bool flag = false;
            using (SqlConnection con = new SqlConnection(MySetting.AppDbSetting.DbConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(MySetting.AppSqlQuery.QDeleteUser, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Id", user.Id);
                  
                    try { con.Open();
                        if (cmd.ExecuteNonQuery() <= 0) {
                            throw new Exception("Unable to delete user. Please Contact System Adminstrator.");
                           
                        }else flag = true;
                    }
                    catch (Exception ex)
                    {
                        flag = false;
                        throw ex;
                    }
                    finally { con.Close(); }
                }
            }
            return flag;
        }
        public bool   RemoveUserRoles(string userid,string roleid)
        {
             

            bool flag = false;
            using (SqlConnection con = new SqlConnection(MySetting.AppDbSetting.DbConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(MySetting.AppSqlQuery.QRemoveUserRole, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserId", userid);
                    cmd.Parameters.AddWithValue("@RoleId", roleid ); 
                    try { con.Open();
                        if (cmd.ExecuteNonQuery() <= 0) {
                            throw new Exception("Unable to remove user roles. Please Contact System Adminstrator.");
                           
                        }else flag = true;
                    }
                    catch (Exception ex)
                    {
                        flag = false;
                        throw ex;
                    }
                    finally { con.Close(); }
                }
            }
            return flag;
        }
        public bool   AddUserRoles(string userid,string roleid)
        {
             

            bool flag = false;
            using (SqlConnection con = new SqlConnection(MySetting.AppDbSetting.DbConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(MySetting.AppSqlQuery.QAddUserRole, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserId", userid);
                    cmd.Parameters.AddWithValue("@RoleId", roleid ); 
                    try { con.Open();
                        if (cmd.ExecuteNonQuery() <= 0) {
                            throw new Exception("Unable to remove user roles. Please Contact System Adminstrator.");
                           
                        }else flag = true;
                    }
                    catch (Exception ex)
                    {
                        flag = false;
                        throw ex;
                    }
                    finally { con.Close(); }
                }
            }
            return flag;
        }
        public List<MyRole> GetAllRoles()
        {
            List<MyRole> ol = null;
            DataSet ds = new DataSet("data");
            using (SqlConnection con = new SqlConnection(MySetting.AppDbSetting.DbConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(MySetting.AppSqlQuery.QGetAllRoles, con))
                {
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                         da.TableMappings.Add("Table", MyRole.DataTables.dtRoles);
                        da.Fill(ds);
                    }
                }
            }
            if (ds.Tables.Count >= 1)
            {
                ol = (from r in ds.Tables[MyRole.DataTables.dtRoles].AsEnumerable()
                      select new MyRole
                      {
                          RoleId = r.Field<string>(MyRole.Fields.RoleId),
                          RoleName = r.Field<string>(MyRole.Fields.RoleName),
                      }).ToList(); 
            }
            return ol;
        }

    }
}