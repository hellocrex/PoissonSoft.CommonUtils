using System;

namespace PoissonSoft.CommonUtils.Maths
{
    /// <summary>
    /// Утилиты для округления
    /// </summary>
    public static class RoundHelper
    {
        /// <summary>
        /// Проверка того, что значение кратно заданному шагу
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="step">Шаг округления</param>
        /// <returns></returns>
        public static bool CheckStep(decimal value, decimal step)
        {
            return (step > 0) && (value % step == 0);
        }

        private const decimal MAX_VALUE_FOR_ROUND = decimal.MaxValue - 10;

        /// <summary>
        /// Округление вверх до заданного шага
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="step">Шаг округления</param>
        /// <returns></returns>
        public static decimal RoundUpToStep(decimal value, decimal step)
        {
            if (step <= 0 || value <= 0 || value >= MAX_VALUE_FOR_ROUND) return value;
            var adjustedStep = CalcAdjustedStep(value, step);
            return Math.Ceiling(value / adjustedStep) * adjustedStep;
        }

        /// <summary>
        /// Округление вниз до заданного шага
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="step">Шаг округления</param>
        /// <returns></returns>
        public static decimal RoundDownToStep(decimal value, decimal step)
        {
            if (step <= 0 || value <= 0 || value >= MAX_VALUE_FOR_ROUND) return value;
            var adjustedStep = CalcAdjustedStep(value, step);
            return Math.Floor(value / adjustedStep) * adjustedStep;
        }

        /// <summary>
        /// Округление в ближайшую сторону с заданным шагом
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="step">Шаг округления</param>
        /// <returns></returns>
        public static decimal RoundNearestToStep(decimal value, decimal step)
        {
            if (step <= 0 || value <= 0 || value >= MAX_VALUE_FOR_ROUND) return value;
            var adjustedStep = CalcAdjustedStep(value, step);
            return Math.Round(value / adjustedStep) * adjustedStep;
        }

        private static decimal CalcAdjustedStep(decimal value, decimal step)
        {
            var factor = (int)Math.Ceiling(Math.Log10((double) value) - Math.Log10((double) step)) - 28;
            if (factor > 28)
            {
                step *= 1e28m;
                factor -= 28;
            }
            return factor > 0 ? step * (decimal)Math.Pow(10, factor) : step;
        }
    }
}
