using Chip8.Helpers;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8.CPU
{
    public class Register
    {
        private byte[] V = new byte[16];

        public byte this[int index] 
        {
            get
            {
                ValidateRegister(index);
                return V[index];
            }
        }

        public void StoreValue(int register, byte value)
        {
            ValidateRegister(register);
            V[register] = value;
        }
        public void StoreValueOnRegister0xF(byte value) => V[0xF] = value;

        public void IncrementRegisterValue(int register, byte value)
        {
            ValidateRegister(register);
            V[register] += value;
        }

        public void SubtractRegisterValue(int sourceRegister, int destinationRegister)
        {
            ValidateRegister(sourceRegister);
            ValidateRegister(destinationRegister);

            var x = destinationRegister;
            var y = sourceRegister;

            V[x] -= V[y];
            V[0xF] = (byte)(V[x] > V[y] ? 1 : 0);
        }
        public void AddRegisterValueToOther(int sourceRegister, int destinationRegister)
        {
            ValidateRegister(sourceRegister);
            ValidateRegister(destinationRegister);

            var x = destinationRegister;
            var y = sourceRegister;

            V[x] += V[y];
            V[0xF] = (byte)(V[x] + V[y] > 255 ? 1 : 0);
        }
        public void StoreValueOfRegisterInOther(int sourceRegister, int destinationRegister)
        {
            ValidateRegister(sourceRegister);
            ValidateRegister(destinationRegister);

            V[destinationRegister] = V[sourceRegister];
        }

        /// <summary>
        /// Take values of firstRegistor and secondRegistor, do a operation and store value in destination register
        /// </summary>

        public void CalculateRegistersAndStoreValue(int destinationRegister, int firstRegister, int secondRegister, BitOperationType operation)
        {
            ValidateRegister(firstRegister);
            ValidateRegister(secondRegister);
            ValidateRegister(destinationRegister);

            V[destinationRegister] = BitOperation.Calculate(V[firstRegister], V[secondRegister], operation);
        }

        private void ValidateRegister(int register)
        {
            if (register > 0xF)
            {
                throw new ArgumentException("invalid register");
            }
        }


    }
}
