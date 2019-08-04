using System.Threading;
using System.Windows.Forms;

namespace PartyPanelUI
{
    class Program
    {
        static void Main(string[] args)
        {
            PartyPanel panel = new PartyPanel();

            new Thread(() => new Server(panel).Start()).Start();

            Application.Run(panel);
        }
    }
}
