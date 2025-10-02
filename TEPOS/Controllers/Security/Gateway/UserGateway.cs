using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ERP.Controllers.Manager;
using ERP.Models;
using ERP.Models.Security;
using ERP.CSharpLib;
using System.Text;

namespace ERP.Controllers.Security.Gateway
{
    public class UserGateway
    {
        public dynamic GetUsers(ErpManager erpManager)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                var lst = (from usr in database.UserDbSet
                    join cmn in database.CompanyDbSet on usr.CmnCompanyId equals cmn.Id
                    where usr.CmnCompanyId == erpManager.CmnId
                    select new
                    {
                        usr.Id,
                        usr.LoginName,
                        usr.Status,
                        usr.Email,
                        usr.TerminalId,
                        usr.UserCode,
                        Company = cmn.Name

                    }).ToList();

                return lst;
            }
        }

        public int InsertUser(SecUser secUser, ErpManager erpManager)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                secUser.Password = ConvertToBinaryString(secUser.Password);// Adel
                secUser.ConfirmPassword = ConvertToBinaryString(secUser.ConfirmPassword);// Adel
                secUser.CmnCompanyId = erpManager.CmnId;
                secUser.CreateBy = erpManager.UserId;
                secUser.CreateDate = UTCDateTime.BDDateTime();
                database.UserDbSet.Add(secUser);
                return database.SaveChanges();
            }
        }
        // Adel 06/07/2024
        private static string ConvertToBinaryString(string input)
        {
            // Convert the input string to a byte array using UTF-8 encoding
            byte[] bytes = Encoding.UTF8.GetBytes(input);

            // Convert the byte array to a hexadecimal string prefixed with "0x"
            StringBuilder hex = new StringBuilder(bytes.Length * 2 + 2);
            hex.Append("0x");
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }
        public int UpdateUser(SecUser secUser, ErpManager erpManager)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                SecUser user = database.UserDbSet.FirstOrDefault(f => f.Id == secUser.Id);
                if (user != null)
                {
                    user.LoginName = secUser.LoginName;
                    user.Email = secUser.Email;
                    user.TerminalId = secUser.TerminalId;
                    user.ConfirmPassword = ConvertToBinaryString(secUser.ConfirmPassword);// Adel
                    user.Password = ConvertToBinaryString(secUser.Password);// Adel
                    user.Status = secUser.Status;
                    user.UserCode = secUser.UserCode;
                    user.ModifiedBy = erpManager.UserId;
                    user.ModifiedDate = UTCDateTime.BDDateTime();
                    return database.SaveChanges();
                }
                return 0;
            }
        }

        public int DeleteUser(int id)
        {
            using (ConnectionDatabase database = new ConnectionDatabase())
            {
                SecUser user = new SecUser() {Id = id};
                database.Entry(user).State = EntityState.Deleted;
                return database.SaveChanges();
            }
        }




    }
}