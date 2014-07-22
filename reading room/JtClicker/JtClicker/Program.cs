using System;

namespace JtClicker
{
  class Program
  {
    static int Main( string[] args )
    {
      if( 2 != args.Length )
      {
        Console.WriteLine( "usage: JtClicker <dialog caption> <button text>" );
        return 1;
      }
      DialogCloser dc = new DialogCloser( args[0], args[1] );
      return 0;
    }
  }
}
