using Pada1.BBCore.Framework;
using Pada1.BBCore;


namespace BBCore.Conditions
{
    /// <summary>
    /// It is a basic condition to check if two Goal instances have the same value.
    /// </summary>
    [Condition("Basic/CheckGoals")]
    [Help("Checks whether two Goal instances have the same value")]
    public class CheckGoals : ConditionBase
    {
        ///<value>Input First Goal Parameter.</value>
        [InParam("valueA")]
        [Help("First value to be compared")]
        public SquadController.Goal valueA;

        ///<value>Input Second Goal Parameter.</value>
        [InParam("valueB")]
        [Help("Second value to be compared")]
        public SquadController.Goal valueB;

        /// <summary>
        /// Checks whether two Goal have the same value.
        /// </summary>
        /// <returns>the value of compare first boolean with the second boolean.</returns>
		public override bool Check()
        {
            return valueA == valueB;
        }
    }
}