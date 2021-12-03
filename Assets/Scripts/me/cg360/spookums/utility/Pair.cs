
namespace me.cg360.spookums.utility
{
    public class Pair<A, B>
    {

        public A First { get; }
        public B Second { get; }


        public Pair(A first, B second)
        {
            First = first;
            Second = second;
        }

        public static Pair<A, B> Of(A first, B second)
        {
            return new Pair<A, B>(first, second);
        }

    }
}
