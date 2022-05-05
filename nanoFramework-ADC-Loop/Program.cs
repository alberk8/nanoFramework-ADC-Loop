using System;
using System.Device.Adc;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;


namespace nanoFramework_ADC
{
    public class Program
    {
        private static Timer _timer;
        private static GpioController _gpio;
        private static GpioPin _led;
        public static void Main()
        {
            AdcController adc1;
            AdcChannel ac0 = null;
            AdcChannel ac3 = null;
            int maxAdc;
            int minAdc;
            const ushort averages = 50;
            ushort AdcCount = 0;
            float Dp, Temp;
            double adc1Sum = 0;
            double adc3Sum = 0;
            ushort uDp = 0, uTemp = 0;
            object obj = new object();
            ushort[] Adc1_0Values = new ushort[averages];
            ushort[] Adc1_3Values = new ushort[averages];
            ushort Adc1_0Value = 0;
            ushort Adc1_3Value = 0;

            _gpio = new GpioController();
            _led = _gpio.OpenPin(21, PinMode.OutputOpenDrainPullUp);

            var ledTimer = new LedTimer();

            Thread thread = new Thread(new ThreadStart(ledTimer.StartTimer));

            thread.Start();

            //Debug.WriteLine("Before ADC Controller");

            adc1 = new AdcController
            {
                ChannelMode = AdcChannelMode.SingleEnded
            };
            maxAdc = adc1.MaxValue;
            minAdc = adc1.MinValue;
            ac0 = adc1.OpenChannel(0);
            ac3 = adc1.OpenChannel(3);
            try
            {
                //Debug.WriteLine("Before While");
                while (true)
                {
                    Adc1_0Value = (ushort)ac0.ReadValue();
                    Adc1_3Value = (ushort)ac3.ReadValue();

                    Adc1_0Values[AdcCount] = Adc1_0Value;
                    Adc1_3Values[AdcCount] = Adc1_3Value;

                    AdcCount++; if (AdcCount >= averages) { AdcCount = 0; }

                    adc1Sum = adc3Sum = 0;                       // this section causes hang
                    for (byte i = 0; i < averages; i++)
                    {
                        adc1Sum += Adc1_0Values[i];
                        adc3Sum += Adc1_3Values[i];
                    }
                    adc1Sum = adc1Sum / averages;
                    adc3Sum = adc3Sum / averages;          // this section causes hang
                    //Debug.WriteLine($"Count: {AdcCount} ADC1: {Adc1_0Value} ADC2: {Adc1_3Value} ");
                    // Debug.WriteLine($"Count: {AdcCount} ");

                    // NativeMemory.GetMemoryInfo(NativeMemory.MemoryType.Internal, out uint totalSize, out uint freeSize, out uint largestBlock);
                    //Debug.WriteLine($"FreeSize: {freeSize} LargestBlock: {largestBlock}");

                    //Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public class LedTimer
        {
            public void StartTimer()
            {
                while (true)
                {
                    _led.Toggle();
                    Thread.Sleep(100);
                }
            }
        }
    }
}

