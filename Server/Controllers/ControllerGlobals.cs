namespace Server.Controllers
{
    public class ControllerGlobals
    {
        public static DatabaseController dbControl;
        public static Dictionary<string, string> confirmInProcess;

        static ControllerGlobals()
        {
            dbControl = new DatabaseController();
            confirmInProcess = new Dictionary<string, string>();
        }
    }
}
