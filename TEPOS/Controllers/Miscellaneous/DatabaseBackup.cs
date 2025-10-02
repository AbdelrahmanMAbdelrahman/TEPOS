using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using ERP.Models;
using ERP.Models.VModel;

using ERP.CSharpLib;

namespace ERP.Controllers.Miscellaneous
{
    public class DatabaseBackup
    {
        public dynamic GetDriverList()
        {

            List<string[]> list = new List<string[]>();

            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                //if (d.Name.Remove(1, 2) != "C")
                //{
                list.Add(new[] {d.Name, d.Name.Remove(1, 2)});
                //}
            }
            var drList = (from dl in list
                select new
                {
                    Id = dl[0],
                    Name = dl[1]
                }).ToList();
            return drList;

        }


        public dynamic GetDatabaseList()
        {

            using (ConnectionDatabase dbContext = new ConnectionDatabase())
            {
                List<string> ignoDbList = new List<string>
                {
                    "master",
                    "model",
                    "msdb",
                    "tempdb"
                };
                List<string> dbName = dbContext.Database.SqlQuery<string>("SELECT name from sys.databases").ToList();
                var rtr = (from d in dbName
                    where !ignoDbList.Contains(d)
                    select new
                    {
                        Id = d,
                        Name = d
                    }).ToList();

                return rtr;
                
            }
        }

        public int BackupProsecss(string drive, string database)
        {
            string backupName = UTCDateTime.BDDateTime().ToString("ddMMyyyy_HHmmss");
            string path = drive + "RMPOS_Database_Backup\\" + database + "_" + backupName + ".bak";
            string servPath = drive + "RMPOS_Database_Backup";

            bool exists = Directory.Exists(servPath);
            if (!exists)
            {
                Directory.CreateDirectory(servPath);
            }
            using (var conn = new ConnectionDatabase())
            {
                conn.Database.CommandTimeout = 360;
                conn.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction,"BACKUP DATABASE " + database + " TO DISK='" + path+"'");
                return 1;
            }
        }

        public int BackupProsecssForMdf(string drive, string database)
        {
            string backupName = UTCDateTime.BDDateTime().ToString("ddMMyyyy_HHmmss");
            string path = drive + "RMPOS_Database_Backup\\" + database + "_" + backupName + ".mdf";
            string servPath = drive + "RMPOS_Database_Backup";

            bool exists = Directory.Exists(servPath);
            if (!exists)
            {
                Directory.CreateDirectory(servPath);
            }

            using (var conn = new ConnectionDatabase())
            {
                VmDatabaseBackup fileFromDatabase =conn.Database.SqlQuery<VmDatabaseBackup>("SELECT top(1) physical_name as Path,name as DB FROM sys.database_files where type=0").FirstOrDefault<VmDatabaseBackup>();

                //Copy all the files & Replaces any files with the same name

                if (System.IO.File.Exists(servPath))
                {
                    System.IO.File.Delete(servPath);
                }

                System.IO.File.Copy(fileFromDatabase.Path, servPath+"\\"+ fileFromDatabase.DB+".mdf", true);

                if (fileFromDatabase != null)
                {
                    //rename file
                    System.IO.File.Move(servPath + "\\" + fileFromDatabase.DB + ".mdf", path);
                }
                return 1;
            }
        }


    }
}