﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Neuro_test.TwoLayersNetwork
{
    public class NeuronWithActivationFunction : ISignalProvider, IErrorProvider
    {
        private readonly IList<ISignalProvider> inputSignalFromNeuro;
        private double delta; //ошибка
        public IList<IErrorProvider> NextLayer;

        //public double SignalOUT;

        private double sum; //e = input_signal*weight + ...;
        private IList<double> weight; //веса в него входящих
        private static Random rnd = new Random(10);

        //public void CalculateSignalOUT()
        //{
        //    sum = 0;
        //    for (var i = 0; i < inputSignalFromNeuro.Count; i++)
        //    {
        //        sum += inputSignalFromNeuro[i].Signal * weight[i];
        //    }
        //    SignalOUT = Sigmoid(sum);
        //}

        public NeuronWithActivationFunction(IList<ISignalProvider> inputSignalFromNeuroOut)
        {
            this.inputSignalFromNeuro = inputSignalFromNeuroOut;
            weight = new List<double>();
            for (int i = 0; i<inputSignalFromNeuroOut.Count; i++)
            {
                weight.Add(rnd.NextDouble()-0.5);
                //weight.Add(0.5);
            }
        }

        public NeuronWithActivationFunction(IList<ISignalProvider> inputSignalFromNeuro, IList<double> weight)
            // IList<NeuronWithActivationFunction> nextLayer)
        {
            this.inputSignalFromNeuro = inputSignalFromNeuro;
            this.weight = weight;
            //NextLayer = nextLayer;
        }

        private double GetSum
        {
            get
            {
                double sum = 0;
                for (var i = 0; i < inputSignalFromNeuro.Count; i++)
                {
                    sum += inputSignalFromNeuro[i].Signal*weight[i];
                }
                return sum;
            }
        }

        public double Signal
        {
            get
            {
                sum = GetSum;
                return Sigmoid(sum);
            }
        }

        public double GetWeightedError(NeuronWithActivationFunction neuron)
        {
            return delta*GetWeight(neuron);
        }
        //тестовая часть
        //public void RecalculateDeltaOutput(double Error)
        //{
        //    for (int i = 0; i<10; i++)
        //    {
        //        delta = Error*SigmoidDerivative(sum);
        //    }
        //}
        //public void RecalculateDeltaHidden()
        //{
        //    double newDelta = 0;
        //    foreach (var rightNeuron in NextLayer)
        //    {
        //        newDelta += rightNeuron.GetWeightedError(this);
        //    }
        //    delta = newDelta*SigmoidDerivative(sum);
        //}

        //public void RecalculateWeightsAnother(double speed)
        //{
        //    for (var i = 0; i < weight.Count; i++)
        //    {
        //        weight[i] = weight[i] + speed * delta * Sigmoid(sum);
        //    }
        //}
        //тестовая часть
        public void RecalculateDelta()
        {
            double newDelta = 0;
            foreach (var rightNeuron in NextLayer)
            {
                newDelta += rightNeuron.GetWeightedError(this);
            }
            delta = newDelta;
        }

        public void RecalculateWeights(double speed)
        {
            for (var i = 0; i < weight.Count; i++)
            {
                weight[i] =weight[i] + speed*delta*SigmoidDerivative(sum)*inputSignalFromNeuro[i].Signal;
            }
        }

        public string SaveToJson()
        {
            return JsonConvert.SerializeObject(weight);
        }

        public void RestoreFromJson(string json)
        {
            weight = JsonConvert.DeserializeObject<IList<double>>(json);
        }

        private double Sigmoid(double x)
        {
            return 1/(1 + Math.Exp(-x));
        }

        private double SigmoidDerivative(double x)
        {
            return Sigmoid(x)*(1 - Sigmoid(x));
        }

        private double GetWeight(NeuronWithActivationFunction neuron)
        {
            for (var i = 0; i < inputSignalFromNeuro.Count; i++)
            {
                if (neuron == inputSignalFromNeuro[i])
                {
                    return weight[i];
                }
            }
            throw new ArgumentException("Попытка вычисления ошибки для неизвестного нейрона");
        }
    }
}