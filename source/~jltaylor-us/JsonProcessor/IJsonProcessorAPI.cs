/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewJsonProcessor
**
*************************************************/

// Copyright 2022 Jamie Taylor

// The license for this file (and this file only) is modified to remove the requirement that
// distributions in binary form include the license and copyright notice.  The modified license
// is included below, making this file self-contained.  In other words, it's ok to copy this
// entire file into your mod.

/*
modified BSD 3-Clause License

Copyright(c) 2022, Jamie Taylor
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1.Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. [deleted for this file]

3. Neither the name of the copyright holder nor the names of its
   contributors may be used to endorse or promote products derived from
   this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Linq;

namespace JsonProcessor {
    /// <summary>
    /// The main entry point to the JSON Processor API.
    /// </summary>
    public interface IJsonProcessorAPI {
        /// <summary>
        /// Constructs a new <c cref="IJsonProcessor">IJsonProcessor</c>.
        /// </summary>
        /// <param name="errorLogPrefix">The prefix the processor will use when logging errors</param>
        /// <param name="includeDefaultTransformers">whether to include the default set of transformers</param>
        /// <returns>a new JSON processor</returns>
        IJsonProcessor NewProcessor(string errorLogPrefix, bool includeDefaultTransformers = true);
    }

    /// <summary>
    /// An <c>IJsonProcessor</c> applies transformations to a JSON tree via the <c cref="Transform(JToken)>Transform</c>
    /// method.  The set of transformations is pluggable, and this interface also includes the methods for
    /// adding and removing transformers, as well as some methods used by the transformers themselves.
    /// <para>
    ///   A transformer may or may not support being invoked from multiple processors, so in general it is best
    ///   not to add a transformer to more than one processor.
    /// </para>
    /// </summary>
    public interface IJsonProcessor {
        /********************/
        /* For transforming JSON */
        /********************/

        /// <summary>
        /// Process the given JSON token and its children using the current set of transformers.
        /// </summary>
        /// <param name="tok">the JSON token</param>
        /// <returns>Returns <c>true</c> if no errors were encountered</c></returns>
        bool Transform(JToken tok);

        /********************/
        /* Methods for adding and removing transformers */
        /********************/

        /// <summary>
        /// Add an <c cref="ITransformer>ITransformer</a>.
        /// </summary>
        /// <param name="transformer">the transformer to add</param>
        void AddTransformer(ITransformer transformer);

        /// <summary>
        /// Create an instance of <c cref="ITransformer>ITransformer</c> from the given arguments and add it to the processor.
        /// </summary>
        /// <param name="name">the name of the transformer to create</param>
        /// <param name="transform" >the transformer's <c>TransformNode</c> method</param>
        /// <param name="processChildrenFirst">whether the processor should have already processed child nodes before
        /// invoking <paramref name="transform" /> </param>
        void AddTransformer(string name, Func<IJsonProcessor, JObject, bool> transform, bool processChildrenFirst = true);

        /// <summary>
        /// Add an <c cref="IShorthandTransformer>IShorthandTransformer</c>.
        /// </summary>
        /// <param name="transformer">the transformer to add</param>
        void AddShorthandTransformer(IShorthandTransformer transformer);

        /// <summary>
        /// Create an instance of <c cref="IShorthandTransformer>IShorthandTransformer</c> from the
        /// given arguments and add it to the processor.
        /// </summary>
        /// <param name="name">the name of the transformer to create</param>
        /// <param name="argNameWhenLongForm">the transformer's ArgumentNameWhenLongForm property</param>
        /// <param name="transform">the transformer's <c>TransformValue</c> method</param>
        /// <param name="processArgumentFirst">whether the processor should have already processed the argument token before
        /// invoking <paramref name="transform" /></param>
        void AddShorthandTransformer(string name, string? argNameWhenLongForm, Func<IJsonProcessor, JObject, JToken, bool> transform, bool processArgumentFirst = true);

        /// <summary>
        /// Add an <c cref="IPropertyTransformer">IPropertyTransformer</c>.
        /// </summary>
        /// <param name="transformer">the transformer to add</param>
        void AddPropertyTransformer(IPropertyTransformer transformer);

        /// <summary>
        /// Create an instance of <c cref="IPropertyTransformer>IPropertyTransformer</c> from the
        /// given arguments and add it to the processor.
        /// </summary>
        /// <param name="name">the name of the transformer to create</param>
        /// <param name="transform">the transformer's <c>TransformProperty</c> method</param>
        /// <param name="processArgumentFirst">whether the processor should have already processed the argument token before
        /// invoking <paramref name="transform" /></param>
        void AddPropertyTransformer(string name, Func<IJsonProcessor, JProperty, bool> transform, bool processArgumentFirst = true);

        /// <summary>
        /// Remove any transformers with the given name.
        /// </summary>
        /// <param name="name">name of the transformer(s) to remove</param>
        void RemoveTransformer(string name);

        /********************/
        /* Methods used by transformers */
        /********************/

        /// <summary>
        /// Log an error
        /// </summary>
        /// <param name="tokenPath">the path to the JSON token responsible for the error</param>
        /// <param name="message">the error message</param>
        void LogError(string tokenPath, string message);

        /// <summary>
        /// Set a variable in the processor's global environment
        /// </summary>
        /// <param name="name">the variable name</param>
        /// <param name="value">the variable value</param>
        void SetGlobalVariable(string name, JToken value);

        /// <summary>
        /// Add a set of variable bindings to the processor's environment
        /// </summary>
        /// <param name="bindings">the variable bindings to add</param>
        void PushEnv(IDictionary<string, JToken> bindings);

        /// <summary>
        /// Remove the last set of variable bindings added to the processor's environment
        /// </summary>
        void PopEnv();

        /// <summary>
        /// Attempt to find a variable binding in the processor's environment
        /// </summary>
        /// <param name="name">the variable to find</param>
        /// <param name="value">the value to which the variable is bound</param>
        /// <param name="foundInEnv">an object corresponding to the group of bindings in which the variable was found.
        /// (Useful in detecting recursive expansion.)</param>
        /// <returns>Returns true if the variable was found in the processor's environment</returns>
        bool TryApplyEnv(string name, [MaybeNullWhen(false)] out JToken value, [MaybeNullWhen(false)] out object foundInEnv);
    }

    /// <summary>
    /// A Transformer is applied to nodes of the form
    /// <code>
    /// { "$transform": "NAME"
    ///   // and any additional properties
    /// }
    /// </code>
    /// </summary>
    public interface ITransformer {
        /// <summary>
        /// The name of the transformer
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Whether the processor should have already processed any additional properties in the node before
        /// calling <c cref="TransformNode(IJsonProcessor, JObject)">TransformNode</c>
        /// </summary>
        bool ProcessChildrenFirst { get; }
        /// <summary>
        /// Transform the given JSON node
        /// </summary>
        /// <param name="processor">the processor invoking the transformation</param>
        /// <param name="obj">the node to transform (see interface description)</param>
        /// <returns>returns true if no errors were encountered during the transformation</returns>
        bool TransformNode(IJsonProcessor processor, JObject obj);
    }

    /// <summary>
    /// A Shorthand Transformer is applied to nodes of the form
    /// <code>
    /// { "$NAME": ARG }
    /// </code>
    /// In addition, if <c cref="ArgumentNameWhenLongForm">ArgumentNameWhenLongForm</c> is not null then
    /// registering the shorthand transformer (via
    /// <c cref="IJsonProcessor.AddShorthandTransformer(IShorthandTransformer)">AddShorthandTransformer</c>)
    /// also registers an instance of <c cref="ITransformer>ITransformer</c>
    /// to be invoked on nodes of the form
    /// <code>
    /// { "$transform": "NAME",
    ///   "ARG-NAME": ARG
    /// }
    /// </code>
    /// (where <c>ARG-NAME</c> is the name returned by <c>ArgumentNameWhenLongForm</c>)
    /// </summary>
    public interface IShorthandTransformer {
        /// <summary>
        /// The name of the transformer
        /// </summary>
        string Name { get; }

        /// <summary>
        /// If not null then registering this transformer also registers an <c cref="ITransformer>ITransformer</c>
        /// (see interface description)
        /// </summary>
        string? ArgumentNameWhenLongForm { get; }

        /// <summary>
        /// Whether the processor should have already processed the ARG before calling
        /// <c cref="TransformValue(IJsonProcessor, JObject, JToken)">TransformValue</c>
        /// </summary>
        bool ProcessArgumentFirst { get; }

        /// <summary>
        /// Transform the given JSON node
        /// </summary>
        /// <param name="processor">the processor invoking the transformation</param>
        /// <param name="nodeToReplace">the node to transform (see interface description)</param>
        /// <param name="arg">the ARG for the transformer (see interface description)</param>
        /// <returns>returns true if no errors were encountered during the transformation</returns>
        bool TransformValue(IJsonProcessor processor, JObject nodeToReplace, JToken arg);
    }

    /// <summary>
    /// A Property Transformer is a bit different from the other types of transformers.  Rather than being invoked
    /// on objects of a particular shape, it is invoked on any property named <c>$<i>NAME</i></c> inside of an object.  This is
    /// useful only in limited circumstances.
    /// <para>
    /// Note that there is ambiguity in how an object with a single property matching the NAME is treated.
    /// It could be treated as either a shorthand transformer or property transformer.  If both a
    /// shorthand transformer and property transformer of that name exist then the shorthand transformer is
    /// executed and the property transformer is not.
    /// </para>
    /// </summary>
    public interface IPropertyTransformer {
        /// <summary>
        /// The name of the transformer
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Whether the processor should have already processed the ARG before calling
        /// <c cref="TransformProperty(IJsonProcessor, JProperty)">TransformProperty</c>
        /// </summary>
        bool ProcessArgumentFirst { get; }

        /// <summary>
        /// Transform the given JSON Property
        /// </summary>
        /// <param name="processor">the processor invoking the transformation</param>
        /// <param name="theProperty">the property to transform</param>
        /// <returns>returns true if no errors were encountered during the transformation</returns>
        bool TransformProperty(IJsonProcessor processor, JProperty theProperty);
    }
}

