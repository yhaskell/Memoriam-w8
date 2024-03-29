﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Memoriam
{
    public class Question
    {
        public async static Task<bool> Show(string Message, string Title, string FirstButton = "Yes", string SecondButton="No")
        {
            var msd = new MessageDialog(Message, Title);

            msd.Commands.Add(new UICommand(FirstButton));
            msd.Commands.Add(new UICommand(SecondButton));

            var res = await msd.ShowAsync();

            return (res.Label == FirstButton);            
        }       
    }
}
