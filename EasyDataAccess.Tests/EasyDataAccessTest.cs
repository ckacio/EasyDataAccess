using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDataAccess.Tests
{
    [TestClass]
    public class EasyDataAccessTest
    {
        EasyDataAccess _eda1;
        EasyDataAccess _eda2;
        EasyDataAccess _eda3;
        readonly string _connectionString = "Server=SCORPION-NOTEBO;Database=WorldCartoon;Integrated Security=SSPI;TrustServerCertificate=True;";

        public EasyDataAccessTest()
        {
            //Do you have three way to create a new EasyDataAccess instance

            //Without any parameter
            _eda1 = new EasyDataAccess();
            //With a ConnectionString
            _eda2 = new EasyDataAccess(_connectionString);
            //With a ready connection
            _eda3 = new EasyDataAccess(new SqlConnection(_connectionString));
        }

        [TestMethod]
        public void EasyDataAccess_CreateConnection()
        {
            var db = _eda2.CreateConnection();
            Assert.IsTrue(db.GetType().Name == "EasyDataAccessIntance");
        }

        [TestMethod]
        public async Task EasyDataAccess_CreateConnectionAsync()
        {
            var db = await _eda2.CreateConnectionAsync();
            Assert.IsTrue(db.GetType().Name == "EasyDataAccessIntance");
        }

        [TestMethod]
        public void EasyDataAccess_CreateConnection_Erro()
        {
            //Will get a Exception because the ConnectionString needs to be informed on constructor! Example: new DataAccess(connectionString) or new DataAccess(connection)
            Assert.ThrowsException<System.Exception>(() => _eda1.CreateConnection());
        }

        [TestMethod]
        public void EasyDataAccess_CreateConnection_ConnectionString()
        {
            var db = _eda1.CreateConnection(_connectionString);
            Assert.IsTrue(db.GetType().Name == "EasyDataAccessIntance");
        }

        [TestMethod]
        public async Task EasyDataAccess_CreateConnectionAsync_ConnectionString()
        {
            var db = await _eda1.CreateConnectionAsync(_connectionString);
            Assert.IsTrue(db.GetType().Name == "EasyDataAccessIntance");
        }

        [TestMethod]
        public void EasyDataAccess_CreateConnection_Connection()
        {
            var db = _eda3.CreateConnection();
            Assert.IsTrue(db.GetType().Name == "EasyDataAccessIntance");
        }

        [TestMethod]
        public async Task EasyDataAccess_SetQuery_ExecuteReader()
        {
            var testeOk = true;

            try
            {
                //Creates a new connection
                var db = await _eda2.CreateConnectionAsync();

                //Use to set a query 
                db.SetQuery("Select Cartoon_Id,Name_Cartoon,Country From Cartoon");

                //Traditional way
                var dr = db.ExecuteReader();

                while (dr.Read())
                {
                    Debug.WriteLine(string.Format($"{0}-{1}-{2}", dr["Cartoon_Id"], dr["Name_Cartoon"], dr["Country"]));
                }

                db.CloseConnection();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                testeOk = false;
            }
            finally
            {
                Assert.IsTrue(testeOk);
            }
        }

        [TestMethod]
        public async Task EasyDataAccess_SetStoredProcedure_ExecuteReader()
        {
            var testeOk = true;

            try
            {
                //Creates a new connection
                var db = await _eda2.CreateConnectionAsync();

                //Use to set a query 
                db.SetStoredProcedure("SEL_Cartoon");

                //Traditional way
                var dr = db.ExecuteReader();

                while (dr.Read())
                {
                    Debug.WriteLine(string.Format($"{0}-{1}-{2}", dr["Cartoon_Id"], dr["Name_Cartoon"], dr["Country"]));
                }

                db.CloseConnection();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                testeOk = false;
            }
            finally
            {
                Assert.IsTrue(testeOk);
            }
        }

        [TestMethod]
        public async Task EasyDataAccess_SetQuery_ExecuteReaderAsync()
        {
            var testeOk = true;

            try
            {
                //Creates a new connection
                var db = await _eda2.CreateConnectionAsync();

                //Use to set a query 
                db.SetQuery("Select Cartoon_Id,Name_Cartoon,Country From Cartoon");

                //Traditional way
                var dr = await db.ExecuteReaderAsync();

                while (dr.Read())
                {
                    Debug.WriteLine(string.Format($"{0}-{1}-{2}", dr["Cartoon_Id"], dr["Name_Cartoon"], dr["Country"]));
                }

                db.CloseConnection();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                testeOk = false;
            }
            finally
            {
                Assert.IsTrue(testeOk);
            }
        }

        [TestMethod]
        public async Task EasyDataAccess_SetStoredProcedure_ExecuteReaderAsync()
        {
            var testeOk = true;

            try
            {
                //Creates a new connection
                var db = await _eda2.CreateConnectionAsync();

                //Use to set a query 
                db.SetStoredProcedure("SEL_Cartoon");

                //Traditional way
                var dr = await db.ExecuteReaderAsync();

                while (dr.Read())
                {
                    Debug.WriteLine(string.Format($"{0}-{1}-{2}", dr["Cartoon_Id"], dr["Name_Cartoon"], dr["Country"]));
                }

                db.CloseConnection();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                testeOk = false;
            }
            finally
            {
                Assert.IsTrue(testeOk);
            }
        }

        [TestMethod]
        public async Task EasyDataAccess_SetQuery_ExecuteReaderAsyncT()
        {
            var testeOk = true;

            try
            {
                //Creates a new connection
                var db = await _eda2.CreateConnectionAsync();

                //Use to set a query 
                db.SetQuery("Select Cartoon_Id,Name_Cartoon,Country From Cartoon");

                //Use to force map between entity field and database field returned. Only if necessary where they are different! 
                db.SetMap("Name_Cartoon", "Name");

                //If the entity field and the database field are the same, the mapping will be automatic.
                //It compares by removing the "_" signs and putting everything in lowercase and compares to see if they are the same
                //(entity) NameCartoon == (database) Name_cartoon = Are Iqual in this case
                var cartoons = await db.ExecuteReaderAsync<Cartoon>();

                foreach (var cartoon in cartoons)
                {
                    Debug.WriteLine(string.Format($"{0}-{1}-{2}", cartoon.CartoonId, cartoon.Name, cartoon.Country));
                }

                db.CloseConnection();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                testeOk = false;
            }
            finally
            {
                Assert.IsTrue(testeOk);
            }
        }

        [TestMethod]
        public async Task EasyDataAccess_SetStoredProcedure_ExecuteReaderAsyncT()
        {
            var testeOk = true;

            try
            {
                //Creates a new connection
                var db = await _eda2.CreateConnectionAsync();

                //Use to set a query 
                db.SetStoredProcedure("SEL_Cartoon");

                //Use to force map between entity field and database field returned. Only if necessary where they are different! 
                db.SetMap("Name_Cartoon", "Name");

                //If the entity field and the database field are the same, the mapping will be automatic.
                //It compares by removing the "_" signs and putting everything in lowercase and compares to see if they are the same
                //(entity) NameCartoon == (database) Name_cartoon = Are Iqual in this case
                var cartoons = await db.ExecuteReaderAsync<Cartoon>();

                foreach (var cartoon in cartoons)
                {
                    Debug.WriteLine(string.Format($"{0}-{1}-{2}", cartoon.CartoonId, cartoon.Name, cartoon.Country));
                }

                db.CloseConnection();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                testeOk = false;
            }
            finally
            {
                Assert.IsTrue(testeOk);
            }
        }

        [TestMethod]
        public void EasyDataAccess_SetQuery_ExecuteReaderT_WithParameters()
        {
            var testeOk = true;

            try
            {
                var db = _eda2.CreateConnection();

                //Use to set a query 
                db.SetQuery("Select Cartoon_Id,Name_Cartoon,Country From Cartoon where Cartoon_Id = @Id ");

                //Use to set parameter to query e stored procedures 
                db.SetParameter("@Id", "2");

                //Use to force map between entity field and database field returned. Only if necessary where they are different!
                db.SetMap("Name_Cartoon", "Name");

                //Use to fixed a value on entity field returned. Only if necessary!  
                db.SetFixedValue("Country", "BR");

                //If the entity field and the database field are the same, the mapping will be automatic.
                //It compares by removing the "_" signs and putting everything in lowercase and compares to see if they are the same
                //(entity) NameCartoon == (database) Name_cartoon = Are Iqual in this case
                var cartoons = db.ExecuteReader<Cartoon>();

                foreach (var cartoon in cartoons)
                {
                    Debug.WriteLine(string.Format($"{0}-{1}-{2}", cartoon.CartoonId, cartoon.Name, cartoon.Country));
                }

                db.CloseConnection();
            }
            catch
            {
                testeOk = false;
            }
            finally
            {
                Assert.IsTrue(testeOk);
            }
        }

        [TestMethod]
        public void EasyDataAccess_SetStoredProcedure_ExecuteReaderT_WithParameters()
        {
            var testeOk = true;

            try
            {
                var db = _eda2.CreateConnection();

                //Use to set a query 
                db.SetStoredProcedure("SEL_Cartoon");

                //Use to set parameter to query e stored procedures 
                db.SetParameter("@Country", "USA");

                //Use to force map between entity field and database field returned. Only if necessary where they are different!
                db.SetMap("Name_Cartoon", "Name");

                //If the entity field and the database field are the same, the mapping will be automatic.
                //It compares by removing the "_" signs and putting everything in lowercase and compares to see if they are the same
                //(entity) NameCartoon == (database) Name_cartoon = Are Iqual in this case
                var cartoons = db.ExecuteReader<Cartoon>();

                foreach (var cartoon in cartoons)
                {
                    Debug.WriteLine(string.Format($"{0}-{1}-{2}", cartoon.CartoonId, cartoon.Name, cartoon.Country));
                }

                db.CloseConnection();
            }
            catch
            {
                testeOk = false;
            }
            finally
            {
                Assert.IsTrue(testeOk);
            }
        }

        [TestMethod]
        public async Task EasyDataAccess_SetQuery_ExecuteReaderAsyncT_WithParameters()
        {
            var testeOk = true;

            try
            {
                var db = await _eda2.CreateConnectionAsync();

                //Use to set a query 
                db.SetQuery("Select Cartoon_Id,Name_Cartoon,Country From Cartoon where Cartoon_Id = @Id ");

                //Use to set parameter to query e stored procedures 
                db.SetParameter("@Id", "2");

                //Use to force map between entity field and database field returned. Only if necessary where they are different!
                db.SetMap("Name_Cartoon", "Name");

                //Use to fixed a value on entity field returned. Only if necessary!  
                db.SetFixedValue("Country", "ESP");

                //If the entity field and the database field are the same, the mapping will be automatic.
                //It compares by removing the "_" signs and putting everything in lowercase and compares to see if they are the same
                //(entity) NameCartoon == (database) Name_cartoon = Are Iqual in this case
                var cartoons = await db.ExecuteReaderAsync<Cartoon>();

                foreach (var cartoon in cartoons)
                {
                    Debug.WriteLine(string.Format($"{0}-{1}-{2}", cartoon.CartoonId, cartoon.Name, cartoon.Country));
                }

                db.CloseConnection();
            }
            catch
            {
                testeOk = false;
            }
            finally
            {
                Assert.IsTrue(testeOk);
            }
        }

        [TestMethod]
        public async Task EasyDataAccess_SetStoredProcedure_ExecuteReaderAsyncT_WithParameters()
        {
            var testeOk = true;

            try
            {
                var db = await _eda2.CreateConnectionAsync();

                //Use to set a query 
                db.SetStoredProcedure("SEL_CartoonCharacter");

                //Use to set parameter to query e stored procedures 
                db.SetParameter("@Cartoon_Id", "1");

                //If the entity field and the database field are the same, the mapping will be automatic.
                //It compares by removing the "_" signs and putting everything in lowercase and compares to see if they are the same
                //(entity) NameCartoon == (database) Name_cartoon = Are Iqual in this case
                var cartoons = await db.ExecuteReaderAsync<CartoonCharacter>();

                foreach (var cartoon in cartoons)
                {
                    Debug.WriteLine(string.Format($"{0}-{1}-{2}", cartoon.CartoonId, cartoon.NameCharacter, cartoon.Description));
                }

                db.CloseConnection();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                testeOk = false;
            }
            finally
            {
                Assert.IsTrue(testeOk);
            }
        }

        [TestMethod]
        public void EasyDataAccess_SetQuery_Insert_WithParameters_ExecuteNoQuery()
        {
            var testeOk = true;

            try
            {
                var db = _eda2.CreateConnection();

                //Use to set a query 
                db.SetQuery(@"insert into Cartoon (Name_Cartoon,Country) 
                            values( @Name,@Country ) ");

                //Use to set parameter to query e stored procedures 
                db.ClearParameters();
                db.SetParameter("@Name", "Jaspion");
                db.SetParameter("@Country", "Japan");

                //Returns the number of rows affected
                var rowsAffected = db.ExecuteNonQuery();

                Debug.WriteLine(string.Format("Rows Affected-{0}", rowsAffected));

                db.CloseConnection();
            }
            catch
            {
                testeOk = false;
            }
            finally
            {
                Assert.IsTrue(testeOk);
            }
        }

        [TestMethod]
        public void EasyDataAccess_SetStoredProcedure_Insert_WithParameters_ExecuteNoQuery()
        {
            var testeOk = true;

            try
            {
                var db = _eda2.CreateConnection();

                //Use to set a Stored Procedure 
                db.SetStoredProcedure("INS_Cartoon");

                //Use to set parameter to query e stored procedures 
                db.ClearParameters();
                db.SetParameter("Name", "Dragon Ball");
                db.SetParameter("Country", "Japan");

                //Use to set parameter output to your stored procedure 
                db.SetParameterOutput("CartoonId", DbType.Int32);

                //Returns the number of rows affected
                var rowsAffected = db.ExecuteNonQuery();

                var cartoonId = db.GetParameterOuput("CartoonId");

                Debug.WriteLine(string.Format("Rows Affected-{0} Created CartoonId-{1}", rowsAffected, cartoonId));

                db.CloseConnection();
            }
            catch
            {
                testeOk = false;
            }
            finally
            {
                Assert.IsTrue(testeOk);
            }
        }

        [TestMethod]
        public async Task EasyDataAccess_SetQuery_Insert_WithParameters_ExecuteNonQueryAsync()
        {
            var testeOk = true;

            try
            {
                var db = _eda2.CreateConnection();

                //Use to set a query 
                db.SetQuery(@"insert into Cartoon (Name_Cartoon,Country) 
                            values( @Name,@Country ) ");

                //Use to set parameter to query e stored procedures 
                db.ClearParameters();
                db.SetParameter("@Name", "Jaspion");
                db.SetParameter("@Country", "Japan");

                //Returns the number of rows affected
                var rowsAffected = await db.ExecuteNonQueryAsync();

                Debug.WriteLine(string.Format("Rows Affected-{0}", rowsAffected));

                db.CloseConnection();
            }
            catch
            {
                testeOk = false;
            }
            finally
            {
                Assert.IsTrue(testeOk);
            }
        }

        [TestMethod]
        public async Task EasyDataAccess_SetStoredProcedure_Insert_WithParameters_ExecuteNonQueryAsync_Transaction()
        {
            var testeOk = true;

            var db = await _eda2.CreateConnectionAsync();

            try
            {
                db.BeginTransaction();

                //Insert in Cartoon
                db.SetStoredProcedure("INS_Cartoon");
                db.ClearParameters();
                db.SetParameter("@Name", "He-man");
                db.SetParameter("@Country", "EUA");
                db.SetParameterOutput("CartoonId", DbType.Int32);
                db.ExecuteNonQuery();
                var cartoonId = db.GetParameterOuput("CartoonId");

                //Insert in CartoonCharacter
                db.SetStoredProcedure("INS_CartoonCharacter");
                db.ClearParameters();
                db.SetParameter("Cartoon_Id", cartoonId);
                db.SetParameter("Name", "Teela");
                db.SetParameter("Description", "The daughter of Man-At-Arms");
                await db.ExecuteNonQueryAsync();

                db.CommitTransaction();

            }
            catch
            {
                db.RollbackTransaction();

                testeOk = false;
            }

            db.CloseConnection();

            Assert.IsTrue(testeOk);
        }

    }

    public class Cartoon
    {
        public int CartoonId { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
    }

    public class CartoonCharacter
    {
        public int CartoonId { get; set; }
        public int CartoonCharacterId { get; set; }
        public string NameCharacter { get; set; }
        public string Description { get; set; }
    }
}
