using System.Diagnostics;

namespace AsyncCalculator.CalcModel
{
    public class Calculator
    {
        public string CalculateRequest(CalculationRequest request)
        {
            double numBase = Convert.ToDouble(request.Body[..^1]);
            double numOperand = Convert.ToDouble(request.Tail);
            switch (request.Body[^1])
            {
                case '+':
                    return (numBase+numOperand).ToString();
                case '-':
                    return (numBase - numOperand).ToString();
                case '/':
                    try
                    {   
                        if (request.Tail == "0") throw new DivideByZeroException();
                        return (numBase / numOperand).ToString();
                    }
                    catch(DivideByZeroException)
                    {
                        return ("Divided By Zero");
                    }
                case '*':
                    return (numBase * numOperand).ToString();
                default:
                    Debug.WriteLine("No Such Symbol");
                    return ("Error");
            }
        }
    }
}
