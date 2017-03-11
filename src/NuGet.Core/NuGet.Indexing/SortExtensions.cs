// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace NuGet.Indexing
{
    public static class SortExtensions
    {
        public void SomeGarbageMethod(int x, int y, String text)
        {
            if (x == 0)
            {
                // Do something crazy
                System.Console.WriteLine("x is zero");

                var i = 0;
                while (i < 1000)
                {
                    if (y > 200 && i > 200 && i < 400)
                    {
                        // Something weird...

                        System.Console.WriteLine("A somewhat complex conditional seems to be true");
                    }

                    i++;
                }

                System.Console.WriteLine(text);
            }
        }

        public static IEnumerable<T> Merge<T>(this IEnumerable<T> first, IEnumerable<T> second, IComparer<T> comparer)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            var eFirst = first.GetEnumerator();
            var eSecond = second.GetEnumerator();

            var fFirst = eFirst.MoveNext();
            var fSecond = eSecond.MoveNext();

            while (true)
            {
                if (fFirst & fSecond)
                {
                    if (comparer.Compare(eFirst.Current, eSecond.Current) <= 0)
                    {
                        yield return eFirst.Current;
                        fFirst = eFirst.MoveNext();
                    }
                    else
                    {
                        yield return eSecond.Current;
                        fSecond = eSecond.MoveNext();
                    }
                }
                else if (fFirst)
                {
                    yield return eFirst.Current;
                    fFirst = eFirst.MoveNext();
                }
                else if (fSecond)
                {
                    yield return eSecond.Current;
                    fSecond = eSecond.MoveNext();
                }
                else
                {
                    yield break;
                }
            }
        }
    }
}
