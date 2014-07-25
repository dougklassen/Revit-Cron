using System;
using System.Text;
using System.Timers;

namespace JtClicker
{
  /// <summary>
  /// Close a dialogue box by simulating a button click.
  /// </summary>
  public class DialogCloser
  {
    const string m_appTitle = "Mechanical Desktop";
    const string m_popTitle = "Bosch Reorg";
    const int timer_interval = 1000;
    string m_dialog_caption;
    string m_button_text;
    Timer m_timer;

    public DialogCloser( string dialog_caption, string button_text )
    {
      //Console.WriteLine( dialog_caption );
      //Console.WriteLine( button_text );
      m_dialog_caption = dialog_caption;
      m_button_text = button_text;
      m_timer = new Timer( timer_interval );
      m_timer.Elapsed += new ElapsedEventHandler( Timer_Elapsed );
      m_timer.Start();
    }

    private void Timer_Elapsed( object sender, ElapsedEventArgs e )
    {
      int hwnd = WinApi.User32.FindWindow( "", m_dialog_caption );
      Console.WriteLine( hwnd.ToString() );
    }

    private void Timer_Elapsed_jan( object sender, ElapsedEventArgs e )b
    {
      int ret = WinApi.User32.EnumWindows( new WinApi.User32.EnumWindowsProc( EnumWindowsProc ), 0 );
    }

    private bool EnumWindowsProc( int hwnd, int lParam )
    {
      StringBuilder sbTitle = new StringBuilder( 256 );

      WinApi.User32.GetWindowText( hwnd, sbTitle, sbTitle.Capacity );
      string title = sbTitle.ToString();

      if( 0 == title.Length )
      {
        return true;
      }
      int pos = title.IndexOf( '-' );

      if( -1 == pos )
      {
        return true;
      }
      title = title.Substring( 0, pos - 1 );
      if( 0 != string.Compare( m_appTitle, title ) )
      {
        return true;
      }
      int hwndPopup = WinApi.User32.GetLastActivePopup( hwnd );

      WinApi.User32.GetWindowText( hwndPopup, sbTitle, sbTitle.Capacity );
      title = sbTitle.ToString();
      if( m_popTitle != title )
      {
        return true;
      }
      //
      // we found the dialogue, now click the button:
      //
      Console.WriteLine( "JtClicker found it!" );
      m_timer.Stop();
      m_timer = null;

      int ret = WinApi.User32.EnumChildWindows( hwnd, new WinApi.User32.EnumWindowsProc( EnumChildProc ), 0 );

      return false;
    }

    private bool EnumChildProc( int hwnd, int lParam )
    {
      StringBuilder sbTitle = new StringBuilder( 256 );
      WinApi.User32.GetWindowText( hwnd, sbTitle, sbTitle.Capacity );
      string title = sbTitle.ToString();
      if( 0 == title.Length )
      {
        return true;
      }
      //Debug.WriteLine( title );
      if( title != m_button_text )
      {
        return true;
      }
      Console.WriteLine( string.Format( "\nJtClicker found {0}\n", title ) );
      WinApi.User32.SendMessage( hwnd, WinApi.User32.BM_SETSTATE, 1, 0 );
      WinApi.User32.SendMessage( hwnd, WinApi.User32.WM_LBUTTONDOWN, 0, 0 );
      WinApi.User32.SendMessage( hwnd, WinApi.User32.WM_LBUTTONUP, 0, 0 );
      WinApi.User32.SendMessage( hwnd, WinApi.User32.BM_SETSTATE, 1, 0 );
      return false;
    }
  }
}
