using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
	class TextUtility
	{
//        function FormatString ( text : String ) { 
//    words = text.Split(" "[0]); //Split the string into seperate words 
//    result = ""; 
 
//    for( var index = 0; index < words.length; index++)
//    { 
//       var word = words[index].Trim(); 
//       if (index == 0) {
//         result = words[0]; 
//         block.text = result; 
//       } 
 
//       if (index > 0 ) { 
//         result += " " + word; 
//         block.text = result; 
//       } 
//       TextSize = block.GetScreenRect(); 
//       if (TextSize.width > lineLength)
//       { 
//         //remover 
//         result = result.Substring(0,result.Length-(word.Length)); 
//         result += "\n" + word; 
//         numberOfLines += 1;
//         block.text = result;
//       } 
//    } 
//}

        /// <summary>
        /// Checks whether the given tag equals one of the Tags.
        /// </summary>
        /// <param name="Tags">Tags to check against</param>
        /// <param name="tag">Tag to check against</param>
        /// <returns>True if Tag is part of Tags or Tags is empty, false otherwise</returns>
        public static bool CheckIfTagsMatch(string[] Tags, string tag)
        {
            if (Tags.Length > 0)
            {
                bool rightTag = false;

                for (int i = 0; i < Tags.Length; i++)
                {
                    if (Tags[i] == tag)
                    {
                        rightTag = true;
                        break;
                    }
                }

                if (!rightTag)
                    return false;
            }
            return true;
        }
	}
}
