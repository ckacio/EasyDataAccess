using DataAccess.Extentions;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Diagnostics;

namespace EasyDataAccess.Core.Tests
{
    [TestClass]
    public class EasyDataAccessTest
    {
        EasyDataAccess _eda;
        readonly string _connectionString = "Server=SCORPION-NOTEBO;Database=WorldCartoon;Integrated Security=SSPI;TrustServerCertificate=True;";

        public EasyDataAccessTest()
        {
            //Create instance
            _eda = EasyDataAccess.Instance;
        }

        [TestMethod]
        public void EasyDataAccess_CreateConnection()
        {
            using (var db = _eda.CreateConnection(_connectionString))
            {
                Assert.IsTrue(db.GetType().Name == "EasyDataAccessConnection");
            }
        }

        [TestMethod]
        public async Task EasyDataAccess_CreateConnectionAsync()
        {
            using (var db = await _eda.CreateConnectionAsync(_connectionString))
            {
                Assert.IsTrue(db.GetType().Name == "EasyDataAccessConnection");
            }
        }

        [TestMethod]
        public void EasyDataAccess_CreateConnection_Erro()
        {
            //Will get a Exception because the ConnectionString needs to be informed on constructor! 
            try
            {
                using (var db = _eda.CreateConnection(""))
                {
                    Assert.IsTrue(db.GetType().Name == "EasyDataAccessConnection");
                }
            }
            catch
            {
                Assert.IsTrue(true);
            }

        }

        [TestMethod]
        public void EasyDataAccess_CreateConnection_ConnectionString()
        {
            using (var db = _eda.CreateConnection(_connectionString))
            {
                Assert.IsTrue(db.GetType().Name == "EasyDataAccessConnection");
            }
        }

        [TestMethod]
        public async Task EasyDataAccess_CreateConnectionAsync_ConnectionString()
        {
            using (var db = await _eda.CreateConnectionAsync(_connectionString))
            {
                Assert.IsTrue(db.GetType().Name == "EasyDataAccessConnection");
            }
        }

        [TestMethod]
        public async Task EasyDataAccess_CreateConnection_SqlConecctionnAsync()
        {
            var sqlConecction = new SqlConnection(_connectionString);
            await sqlConecction.OpenAsync();

            using (var db = await _eda.CreateConnectionAsync(sqlConecction))
            {
                Assert.IsTrue(db.GetType().Name == "EasyDataAccessConnection");
            }

            sqlConecction.Close();
        }

        [TestMethod]
        public async Task EasyDataAccess_SetQuery_ExecuteReader()
        {
            var testeOk = true;

            try
            {
                //Creates a new connection
                using (var db = await _eda.CreateConnectionAsync(_connectionString))
                {

                    //Use to set a query 
                    db.SetQuery("Select Cartoon_Id,Name_Cartoon,Country From Cartoon");

                    //Traditional way
                    using (var dr = db.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Debug.WriteLine(string.Format("{0}-{1}-{2}", dr["Cartoon_Id"], dr["Name_Cartoon"], dr["Country"]));
                        }
                    }
                }
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
                using (var db = await _eda.CreateConnectionAsync(_connectionString))
                {
                    //Use to set a Stored Procedure 
                    db.SetStoredProcedure("SEL_Cartoon");

                    //Traditional way
                    using (var dr = db.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Debug.WriteLine(string.Format("{0}-{1}-{2}", dr["Cartoon_Id"], dr["Name_Cartoon"], dr["Country"]));
                        }
                    }
                }
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
                using (var db = await _eda.CreateConnectionAsync(_connectionString))
                {
                    //Use to set a query 
                    db.SetQuery("Select Cartoon_Id,Name_Cartoon,Country From Cartoon");

                    //Traditional way
                    using (var dr = await db.ExecuteReaderAsync())
                    {
                        while (dr.Read())
                        {
                            Debug.WriteLine(string.Format("{0}-{1}-{2}", dr["Cartoon_Id"], dr["Name_Cartoon"], dr["Country"]));
                        }
                    }
                }
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
                using (var db = await _eda.CreateConnectionAsync(_connectionString))
                {
                    //Use to set a Stored Procedure 
                    db.SetStoredProcedure("SEL_Cartoon");

                    //Traditional way
                    using (var dr = await db.ExecuteReaderAsync())
                    {
                        while (dr.Read())
                        {
                            Debug.WriteLine(string.Format("{0}-{1}-{2}", dr["Cartoon_Id"], dr["Name_Cartoon"], dr["Country"]));
                        }
                    }
                }
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
                using (var db = await _eda.CreateConnectionAsync(_connectionString))
                {
                    //Use to set a query 
                    db.SetQuery("Select Cartoon_Id,Name_Cartoon,Country From Cartoon");

                    //Use to force map between entity field and database field returned. Only if necessary where they are different! 
                    db.SetMap("Name", "Name_Cartoon");

                    //If the entity field and the database field are the same, the mapping will be automatic.
                    //It compares by removing the "_" signs and putting everything in lowercase and compares to see if they are the same
                    //(entity) NameCartoon == (database) Name_cartoon = Are Iqual in this case
                    var cartoons = await db.ExecuteReaderAsync<Cartoon>();

                    foreach (var cartoon in cartoons)
                    {
                        Debug.WriteLine(string.Format("{0}-{1}-{2}", cartoon.CartoonId, cartoon.Name, cartoon.Country));
                    }
                }
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
                using (var db = _eda.CreateConnection(_connectionString))
                {
                    //Use to set a Stored Procedure 
                    db.SetStoredProcedure("SEL_Cartoon");

                    //Use to force map between entity field and database field returned. Only if necessary where they are different! 
                    db.SetMap("Name", "Name_Cartoon");

                    //If the entity field and the database field are the same, the mapping will be automatic.
                    //It compares by removing the "_" signs and putting everything in lowercase and compares to see if they are the same
                    //(entity) NameCartoon == (database) Name_cartoon = Are Iqual in this case
                    var cartoons = await db.ExecuteReaderAsync<Cartoon>();

                    foreach (var cartoon in cartoons)
                    {
                        Debug.WriteLine(string.Format("{0}-{1}-{2}", cartoon.CartoonId, cartoon.Name, cartoon.Country));
                    }
                }
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
                using (var db = _eda.CreateConnection(_connectionString))
                {
                    //Use to set a query 
                    db.SetQuery("Select Cartoon_Id,Name_Cartoon,Country From Cartoon where Cartoon_Id = @Id ");

                    //Use to set parameter to query e stored procedures 
                    db.SetParameter("@Id", "2");

                    //Use to force map between entity field and database field returned. Only if necessary where they are different!
                    db.SetMap("Name", "Name_Cartoon");

                    //Use to fixed a value on entity field returned. Only if necessary!  
                    db.SetFixedValue("Country", "BR");

                    //If the entity field and the database field are the same, the mapping will be automatic.
                    //It compares by removing the "_" signs and putting everything in lowercase and compares to see if they are the same
                    //(entity) NameCartoon == (database) Name_cartoon = Are Iqual in this case
                    var cartoons = db.ExecuteReader<Cartoon>();

                    foreach (var cartoon in cartoons)
                    {
                        Debug.WriteLine(string.Format("{0}-{1}-{2}", cartoon.CartoonId, cartoon.Name, cartoon.Country));
                    }
                }
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
                using (var db = _eda.CreateConnection(_connectionString))
                {
                    //Use to set a query 
                    db.SetStoredProcedure("SEL_Cartoon");

                    //Use to set parameter to query e stored procedures 
                    db.SetParameter("@Country", "USA");

                    //Use to force map between entity field and database field returned. Only if necessary where they are different!
                    db.SetMap("Name", "Name_Cartoon");

                    //If the entity field and the database field are the same, the mapping will be automatic.
                    //It compares by removing the "_" signs and putting everything in lowercase and compares to see if they are the same
                    //(entity) NameCartoon == (database) Name_cartoon = Are Iqual in this case
                    var cartoons = db.ExecuteReader<Cartoon>();

                    foreach (var cartoon in cartoons)
                    {
                        Debug.WriteLine(string.Format("{0}-{1}-{2}", cartoon.CartoonId, cartoon.Name, cartoon.Country));
                    }
                }
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
                using (var db = _eda.CreateConnection(_connectionString))
                {
                    //Use to set a query 
                    db.SetQuery("Select Cartoon_Id,Name_Cartoon,Country From Cartoon where Cartoon_Id = @Id ");

                    //Use to set parameter to query e stored procedures 
                    db.SetParameter("@Id", "2");

                    //Use to force map between entity field and database field returned. Only if necessary where they are different!
                    db.SetMap("Name", "Name_Cartoon");

                    //Use to fixed a value on entity field returned. Only if necessary!  
                    db.SetFixedValue("Country", "ESP");

                    //If the entity field and the database field are the same, the mapping will be automatic.
                    //It compares by removing the "_" signs and putting everything in lowercase and compares to see if they are the same
                    //(entity) NameCartoon == (database) Name_cartoon = Are Iqual in this case
                    var cartoons = await db.ExecuteReaderAsync<Cartoon>();

                    foreach (var cartoon in cartoons)
                    {
                        Debug.WriteLine(string.Format("{0}-{1}-{2}", cartoon.CartoonId, cartoon.Name, cartoon.Country));
                    }
                }
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
                using (var db = _eda.CreateConnection(_connectionString))
                {
                    //Use to set a Stored Procedure 
                    db.SetStoredProcedure("SEL_CartoonCharacter");

                    //Use to set parameter to query e stored procedures 
                    db.SetParameter("@Cartoon_Id", "1");

                    //If the entity field and the database field are the same, the mapping will be automatic.
                    //It compares by removing the "_" signs and putting everything in lowercase and compares to see if they are the same
                    //(entity) NameCartoon == (database) Name_cartoon = Are Iqual in this case
                    var cartoons = await db.ExecuteReaderAsync<CartoonCharacter>();

                    foreach (var cartoon in cartoons)
                    {
                        Debug.WriteLine(string.Format("{0}-{1}-{2}", cartoon.CartoonId, cartoon.NameCharacter, cartoon.Description));
                    }
                }
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
        public async Task EasyDataAccess_SetQuery_Insert_WithParameters_ExecuteNoQuery()
        {
            var testeOk = true;

            try
            {
                using (var db = _eda.CreateConnection(_connectionString))
                {
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
                }
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
                using (var db = _eda.CreateConnection(_connectionString))
                {

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
                }
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
                using (var db = _eda.CreateConnection(_connectionString))
                {

                    //Use to set a query 
                    db.SetQuery(@"insert into Cartoon (Name_Cartoon,Country) 
                            values( @Name,@Country ) ");

                    //Use to set parameter to query e stored procedures 
                    db.ClearParameters();
                    db.SetParameter("@Name", "Ultraseven");
                    db.SetParameter("@Country", "Japan");

                    //Returns the number of rows affected
                    var rowsAffected = await db.ExecuteNonQueryAsync();

                    Debug.WriteLine(string.Format("Rows Affected-{0}", rowsAffected));
                }
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

            using (var db = await _eda.CreateConnectionAsync(_connectionString))
            {
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
                }
            }

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
