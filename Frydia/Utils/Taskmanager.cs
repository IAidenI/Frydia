using System.Diagnostics;
namespace Frydia.Utils
{
    public class Taskmanager
    {
        private int _blockCount = 0;
        private int _maxBlocks  = -1;
        public bool Acknowledge { get; set; }

        public void Monitor()
        {
            Thread monitorThread = new Thread(() => {
                while (true)
                {
                    Process[] processes = Process.GetProcessesByName("Taskmgr");
                    foreach (Process proc in processes)
                    {
                        this._blockCount++;
                        try
                        {
                            proc.Kill();
                            this.Acknowledge = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Erreur : " + ex.Message);
                        }

                        if (this._blockCount >= this._maxBlocks)
                        {
                            return;
                        }
                        break;
                    }
                    Thread.Sleep(500);
                }
            });

            monitorThread.IsBackground = true;
            monitorThread.Start();
        }
    }
}
