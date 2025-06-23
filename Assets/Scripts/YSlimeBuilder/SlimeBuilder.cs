using UnityEngine;
using BuilderPattern.Services.Abstractions;
namespace BuilderPattern.Services.Implementations
{
    public class SlimeBuilder : MonoBehaviour, ISlimeBuilder
    {
        /* what is this builder gonna build
         * we will need slime cores
         * slime exteriors
         * slime joints
         * 
         * we should first accept inputs in the form of 
         * shape
         * vertices
         * size
         * 
         * we will also need some type of cleanup im assuming, to reset instruction
         */

 

        //lets make a set of general methods for all builders
        public void ProcessShape(int shape, float size)
        {

        }

        public void PopulatePoints()
        {

        }

        public void BuildSlime()
        {

        }

        #region concrete builders
        #endregion

    }
}
