using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineStatusChanger
{
    class OdbcDbIf
    {
        private OdbcConnection _con = null;

        private OdbcTransaction _trn = null;

        public void Connect(String dsn, String dbn, String uid, String pas, int tot)
        {
            try
            {
                if (_con == null)
                {
                    _con = new OdbcConnection();
                }

                String cst = "";
                cst = cst + ";DSN=" + dsn;
                cst = cst + ";Database=" + dbn;
                cst = cst + ";UID=" + uid;
                cst = cst + ";PWD=" + pas;
                if (tot > -1)
                {
                    cst = cst + ";Connect Timeout=" + tot.ToString();
                }

                _con.ConnectionString = cst;
                _con.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("Connect Error", ex);
            }
        }

        public void Disconnect()
        {
            try
            {
                _con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Disconnect Error", ex);
            }
        }

        public DataTable ExecuteSql(String sql, int tot)
        {
            DataTable dt = new DataTable();

            try
            {
                OdbcCommand sqlCommand = new OdbcCommand(sql, _con, _trn);

                if (tot > -1)
                {
                    sqlCommand.CommandTimeout = tot;
                }

                OdbcDataAdapter adapter = new OdbcDataAdapter(sqlCommand);

                adapter.Fill(dt);
                adapter.Dispose();
                sqlCommand.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("ExecuteSql Error", ex);
            }

            return dt;
        }

        public void BeginTransaction()
        {
            try
            {
                _trn = _con.BeginTransaction();
            }
            catch (Exception ex)
            {
                throw new Exception("BeginTransaction Error", ex);
            }
        }

        public void CommitTransaction()
        {
            try
            {
                if (_trn != null)
                {
                    _trn.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("CommitTransaction Error", ex);
            }
            finally
            {
                _trn = null;
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                if (_trn != null)
                {
                    _trn.Rollback();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("RollbackTransaction Error", ex);
            }
            finally
            {
                _trn = null;
            }
        }

        ~OdbcDbIf()
        {
            Disconnect();
        }
    }
}
