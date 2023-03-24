// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibreHardwareMonitor.Hardware;
using WebSocketSharp;

public class UpdateVisitor : IVisitor
{
    public void VisitComputer(IComputer computer)
    {
        computer.Traverse(this);
    }
    public void VisitHardware(IHardware hardware)
    {
        hardware.Update();
        foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
    }
    public void VisitSensor(ISensor sensor) { }
    public void VisitParameter(IParameter parameter) { }
}

namespace HardwareMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            var ws = new WebSocket("ws://localhost:8080");
            var count = 0;

            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsNetworkEnabled = true,
            };
            computer.Open();
            computer.Accept(new UpdateVisitor());
            // ws.Connect();

            while (count < 200)
            {
                Console.WriteLine("Hello, World!");
                foreach (IHardware hardware in computer.Hardware)
                {
                    hardware.Update();
                    if (hardware.GetType().Name == "NvidiaGpu")
                    {
                        /* 
                        Console.WriteLine("Hardware: {0}, type: {1}", hardware.Name, hardware.GetType().Name);
                        foreach (IHardware subhardware in hardware.SubHardware)
                        {
                            Console.WriteLine("\tSubhardware: {0}", subhardware.Name);
                            foreach (ISensor sensor in subhardware.Sensors)
                            {
                                Console.WriteLine("\t\tSensor: {0}, value: {1}", sensor.Name, sensor.Value);
                            }
                        }
                        */

                        foreach (ISensor sensor in hardware.Sensors)
                        {
                            // if (sensor.Name == "GPU Core")
                            {
                                Console.WriteLine("\tSensor: {0}, value: {1}, type: {2}", sensor.Name, sensor.Value, sensor.SensorType);
                                /* if ((sensor.SensorType).ToString() == "Temperature")
                                {
                                    ws.Send($"{sensor.Name}: {sensor.Value}");
                                } */
                            }
                        }
                    }
                }

                System.Threading.Thread.Sleep(1000);
                count++;
            }
            computer.Close();
            ws.Close();
        }
    }
}