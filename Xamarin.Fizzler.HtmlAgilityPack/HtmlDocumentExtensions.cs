using System;
using System.Collections.Generic;
using global::HtmlAgilityPack;
using System.Diagnostics;

namespace Xamarin.Fizzler.HtmlAgilityPack
{
    /// <summary>
    /// Extensions methods for <see cref="HtmlDocument"/>.
    /// </summary>

    public static class HtmlDocumentExtensions
    {
        private static Dictionary<string, HtmlElementFlag> _defaultElementFlags;

        /// <summary>
        /// Same as <see cref="HtmlDocument.LoadHtml" /> but without the FORM nesting
        /// problem outlined in <a href="http://code.google.com/p/fizzler/issues/detail?id=24">issue #24</a>.
        /// </summary>
        /// <remarks>
        /// This method is designed to be used by the Fizzler project tools 
        /// exclusively and may be removed from a future version.
        /// </remarks>

        public static void LoadHtml2(this HtmlDocument document, string html)
        {
            if (document == null) throw new ArgumentNullException("document");
            document.LoadHtmlWithElementFlags(html, DefaultElementFlags);
        }

        /// <summary>
        /// Same as <see cref="HtmlDocument.Load(string)" /> but without the FORM nesting
        /// problem outlined in <a href="http://code.google.com/p/fizzler/issues/detail?id=24">issue #24</a>.
        /// </summary>
        /// <remarks>
        /// This method is designed to be used by the Fizzler project tools 
        /// exclusively and may be removed from a future version.
        /// </remarks>

        public static void Load2(this HtmlDocument document, string path)
        {
            if (document == null) throw new ArgumentNullException("document");
            document.LoadWithElementFlags(path, DefaultElementFlags);
        }

        /// <summary>
        /// Parses the HTML and loads the document model using supplied
        /// per-element handling options.
        /// </summary>
        /// <remarks>
        /// The behavior of this method is not guaranteed to be thread-safe 
        /// and is primarily a hack around <see cref="HtmlNode.ElementsFlags"/> 
        /// being static.
        /// </remarks>
        /// <remarks>
        /// This method is designed to be used by the Fizzler project tools 
        /// exclusively and may be removed from a future version.
        /// </remarks>

        public static void LoadHtmlWithElementFlags(this HtmlDocument document, string html, IEnumerable<KeyValuePair<string, HtmlElementFlag>> flags)
        {
            if (document == null) throw new ArgumentNullException("document");
            LoadWithElementFlags(flags, () => document.LoadHtml(html));
        }

        /// <summary>
        /// Parses the HTML and loads the document model using supplied
        /// per-element handling options.
        /// </summary>
        /// <remarks>
        /// The behavior of this method is not guaranteed to be thread-safe 
        /// and is primarily a hack around <see cref="HtmlNode.ElementsFlags"/> 
        /// being static.
        /// </remarks>
        /// <remarks>
        /// This method is designed to be used by the Fizzler project tools 
        /// exclusively and may be removed from a future version.
        /// </remarks>

        public static void LoadWithElementFlags(this HtmlDocument document, string path, IEnumerable<KeyValuePair<string, HtmlElementFlag>> flags)
        {
            if (document == null) throw new ArgumentNullException("document");
            LoadWithElementFlags(flags, () => document.Load2(path));
        }

        private delegate void LoadHandler();

        private static void LoadWithElementFlags(IEnumerable<KeyValuePair<string, HtmlElementFlag>> flags, LoadHandler loader)
        {
            Dictionary<string, HtmlElementFlag> oldFlags = null;
            try
            {
                if (flags != null)
                {
                    oldFlags = HtmlNode.ElementsFlags.Clone();
                    HtmlNode.ElementsFlags.Reload(flags);
                }
                loader();
            }
            finally
            {
                if (oldFlags != null)
                    HtmlNode.ElementsFlags.Reload(oldFlags);
            }
        }

        private static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            Debug.Assert(source != null);
            return new Dictionary<TKey, TValue>(source, source.Comparer);
        }

        private static void Reload<TKey, TValue>(this IDictionary<TKey, TValue> target, IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            Debug.Assert(source != null);
            Debug.Assert(target != null);

            target.Clear();
            foreach (var entry in source)
                target[entry.Key] = entry.Value;
        }

        private static Dictionary<string, HtmlElementFlag> DefaultElementFlags
        {
            get
            {
                if (_defaultElementFlags == null)
                {
                    var flags = HtmlNode.ElementsFlags.Clone();
                    // ReSharper disable BitwiseOperatorOnEnumWihtoutFlags
                    flags["form"] = flags["form"] | HtmlElementFlag.Closed;
                    // ReSharper restore BitwiseOperatorOnEnumWihtoutFlags
                    _defaultElementFlags = flags;
                }

                return _defaultElementFlags;
            }
        }
    }
}
