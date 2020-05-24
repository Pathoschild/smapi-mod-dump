# Bag items delivery letters

## Where can I define delivery letters?

In content source `Strings/Mail` (asset `assets/Strings/Mail.json` in original mod)

## How can I define letter strings?

Create a json string record for key prefixed with `bagItemsSentLetter.`, the key structure is:

```yaml
bagItemsSentLetter.<NPC_name> # NPC name - a name of NPC who delivers your forgot items in their bag
```

### Example

```json
{
  "bagItemsSentLetter.Abigail": "Hi {0},^^You forgot some items in my bag, so I've sent them back to you.^I had fun yesterday, I hope to see you again soon!^^    - {1}",
  "bagItemsSentLetter.Maru": "Hi {0},^^Blah blah another bag delivery message    - {1}"
}
```

### Placeholders

Bag delivery letters has these placeholder to use:

- `{0}` - Farmer name
- `{1}` - NPC display name (the NPC who deliver our forgotten items in bag)
