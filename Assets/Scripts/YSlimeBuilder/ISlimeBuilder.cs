using UnityEngine;
namespace BuilderPattern.Services.Abstractions
{
    /* what is an interface for a builder
     * here we specify methods for creating parts of our Product
     * our future concrete builders will implement this interface
     */
    public interface ISlimeBuilder
    { 
        void ProcessShape(int shape, float width);
        void PopulatePoints();


        void BuildSlime();
    }
}
