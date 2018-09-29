# SMAPIGenericShopMod
Adds whatever you want to the shop vendors (Starts with adding animal products to Marnie's shop)

## config.json Setup

Example:
~~~
"shopkeepers": {
    "Marnie": [
      [
        174,
        300
      ],
      [
        182,
        300
      ]
    ]
}
~~~

In this example, we're adding two items to Marnie's shop.
174 (a large white egg) and 182 (a large brown egg), both of which will be sold for 300.

You can also add to multiple shopkeepers by adding another section, like so:

~~~
"shopkeepers": {
    "Marnie": [
      [
        174,
        300
      ]
    ],
    "Pierre": [
      [
        182,
        300
      ]
    ]
}
~~~

Now Marnie will sell the large white eggs, and Pierre the large brown ones. 

All object id data can be found here: http://canimod.com/guides/object-data