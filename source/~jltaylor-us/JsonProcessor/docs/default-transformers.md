**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/jltaylor-us/StardewJsonProcessor**

----

# Default Transformers

These are the transformers included with Json Processor.  They are all included in processors with
`includeDefaultTransformers = true` (which is the default.)

* Variable binding and reference
    * [Define](#define)
    * [Let](#let)
    * [Var](#var) (variable reference)
* Structure manipulation
    * [Splice](#splice)
* Text manipulation
    * [String Join](#string-join)
* Looping
    * [For-each](#for-each)

## Types of transformers

The source JSON file can invoke a transformer in three ways.

1. A regular transformer is invoked via an object with a `$transformer` key whose value is the name
of the transformer.  These tranformers can have any number of required and optional arguments
(supplied as properties inside the same object containing the `$transformer` key).

2. A _shorthand_ transformer is invoked via an object with a single property that is the name of the
transformer prefixed with `$`, and a value that is the single required argument for the transformer.
These transformers take exactly one argument.  They often can also be invoked as regular
transformers with a required argument.

3. A _property_ transformer is invoked via a property that is the name of the transformer prefixed
with `$` appearing inside another object.  (Note that if this is the only property in the object and
there is a shorthand transformer with that name then it will be invoked instead.)

# Variable binding and reference

Each processor maintains an environment in which variable bindings can be resolved. The environment
is either the root (or global) environment, or an "extended" environment.  Both contain variable
bindings, but an extended environment also points to the environment that it is extending.

## Define

The Define transformer sets bindings in the processor's global environment.  The JSON node
that is the Define transformer is removed during transformation.

The transformer can appear as a regular transformer, with a required `bindings` property:

```jsonc
  {
    "$transformer": "define",
    "bindings": ARG
  }
```

as a shorthand transformer:

```jsonc
  { "$define": ARG }
```

or as a property transformer:

```jsonc
  {
    "other properties": "blah, blah",
    "$define": ARG
    // etc
  }
```

The `ARG` must be an object containing the bindings.  Each binding is processed (in order) by
recursively transforming the value then setting the corresponding name in the global environment.
(In other words, the bindings from all previous entries are available when transforming a value)

Note that since the result of the transformation is to remove the node from the JSON tree, if the
transformer appears as the value of an object property then that property is removed from the
transformed output.  For example, the transformed value of

```jsonc
{
  "a": 1,
  "ignore": {
    "$transform": "define",
    "bindings": {}
  },
  "b": 2
}
```

is

```jsonc
{
  "a": 1,
  "b": 2
}
```

## Let

The Let transformer creates an extended environment containing the bindings from the required
`bindings` property during the transformation of the required `body` property.  The JSON node that
is the Let transformer is replaced by the result of transforming the `body`.

For example, the transformed value of

```jsonc
[ {
    "$transform": "let",
    "bindings": { "x": 2 },
    "body": { "$var": "x" }
} ]
```

is
```jsonc
[ 2 ]
```

The bindings introduced in `bindings` are _not_ available during the transformation of other values
in the same bindings object.

## Var

The Var transformer looks up a variable in the processor's current environment and replaces the
transformer node with its value.  No replacement is done if the variable is not found in the
environment, or at the point where a recursive reference is detected.

The Var transformer can appear as a regular transformer with a required `var` property:

```jsonc
{
  "$transform": "var",
  "var": "x"
}
```

or, more likely, as a shorthand transformer:

```jsonc
{ "$var": "x"}
```

# Structure manipulation

## Splice

The Splice transformer inserts its content into the surrounding object or array.  The content must
have a matching type (either object or array).

Splice can appear as a regular transformer with a required `content` property:

```jsonc
[
  1,
  2,
  {
    "$transform": "splice",
    "content": [3, 4]
  },
  5
]
```

as a shorthand transformer 

```jsonc
[
  1,
  2,
  { "$splice": [3, 4] },
  5
]
```

or as a property transformer
```jsonc
{
  "a": 1,
  "$splice": {"b": 2, "c": 3},
  "d": 4
}
```

Note that since the result of the transformation is to replace the node in the JSON tree, if the
transformer appears as the value of an object property then that property is removed from the
transformed output.  For example, the transformed value of

```jsonc
{
  "a": 1,
  "ignore": {
    "$transform": "splice",
    "content": { "b": 2 }
  },
  "c": 3
}
```

is 

```jsonc
{
  "a": 1,
  "b": 2,
  "c": 3
}
```

# Text manipulation

## String Join

The String Join transformer joins an array of strings into a single string, with  an optional
delimiter.  String Join can appear as a regular transformer with a required `strings` property (and
optional `delimiter` property:

```jsonc
{
  "$transform": "string-join",
  "strings": ["a", "b", "c"],
  "delimiter": ", "
}
```

or as a shorthand transformer:

```jsonc
{ "$string-join": [ "a", "b", "c" ] }
```

# Looping

## For-each

The For-each transformer loops over a source array or object, repeatedly expanding its body with new
variable bindings. The For-each JSON node is replaced with an array containing the output. The names of
the required properties match the pronunciaion of this operation as "for each `var` _v_ `in` _source_, `yield`
_body_".

If _source_ is an object then _v_ must be an array of two strings (containing the variable names for
the name and value for the object's properties).  If _source_ is an array then _v_ must be either a
string or an array of one string (the variable name for each array element).

For example:

```jsonc
{
  "$transform": "for-each",
  "var": "x",
  "in": [ 1, 2, 3 ],
  "yield": { "$var": "x" }
}
```

transforms to

```jsonc
[ 1, 2, 3 ]
```
