using System;
using Xunit;
using DbSqlHelper;

namespace DbSqlHelperTest
{
    public class SqlTest
    {
        static SqlTest()
        {
            var connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=SSPI;Initial Catalog=master;";
            Db.AddConnection<System.Data.SqlClient.SqlConnection>(connectionString);
        }

        [Fact]
        public void CommandTest()
        {
            using (var cn = Db.GetConnection())
            using (var command = cn.CreateCommand("select @p0 + @p1",1,2))
            {
                var result = command.ExecuteScalar();
                Assert.Equal(3, result);
            }
        }

        [Fact]
        public void StringSql_Execute()
        {
            "create table #T (ID int,Name nvarchar(20))".SqlExecute();

            var count = @"create table #T (ID int,Name nvarchar(20))
            insert into #T (ID,Name) values (1,@p0),(2,@p1);
            ".SqlExecute("Github","Microsoft");

            Assert.Equal(2, count);
        }

        [Fact]
        public void GetDbConnectionType()
        {
            using (var cn = Db.GetConnection())
            {
                var result = cn.GetDbConnectionType();
                Assert.Equal(DBConnectionType.SqlServer, result);
            }
        }

        [Fact]
        public void ExecuteTest()
        {
            var sql = @"
            with cte as ( 
                select @p0 + @p1 val union all 
                select @p2 + @p3
            ) 
            select * into #T from cte
            ";
            using (var cn = Db.GetConnection())
            {
                var result = cn.ExecuteNonQuery(sql, 1, 2, 3, 4);
                Assert.Equal(2, result);
            }
        }
    }
}
