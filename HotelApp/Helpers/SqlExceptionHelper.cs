using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HotelWeb.Helpers
{
    public class SqlExceptionHelper
    {
        // Detecta violación de FK en SQL Server (error 547)
        public static bool IsForeignKeyViolation(DbUpdateException ex)
        {
            // busca en InnerException y en la base
            if (ex.InnerException is SqlException sql && sql.Number == 547) return true;
            if (ex.GetBaseException() is SqlException b && b.Number == 547) return true;
            return false;
        }
    }
}
