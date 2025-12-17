using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace Test
{
  public class SmartObjectProperty
  {
    public SmartObjectProperty()
    {
    }

    public string Name { get; set; }
    public object Value { get; set; }
    public Type Type { get; set; }
  }
}

