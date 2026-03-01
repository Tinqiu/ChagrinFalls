# Time and Day Conditions

Game content can be gated based on the current time of day and game day. Both conditions support range checking with "between" mode, and additional modes for specific use cases.

---

## TimeOfDay Condition

Gates content based on the current time of day. Supports checking time ranges that wrap around midnight.

### Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `startHour` | int | Yes | — | Starting hour (0-23) |
| `startMinute` | int | No | 0 | Starting minute (0-59) |
| `endHour` | int | No (if `mode="exact"`) | — | Ending hour (0-23) |
| `endMinute` | int | No | 0 | Ending minute (0-59) |
| `mode` | string | No | "between" | Evaluation mode: `"between"` or `"exact"` |

### Modes

#### `between` (default)
Returns `true` if the current time is within the range `[startHour:startMinute, endHour:endMinute)`.

Time ranges that wrap around midnight are supported. For example, `startHour=22` and `endHour=6` evaluates as `true` between 22:00 and 05:59, including times like 23:30 and 04:00.

#### `exact`
Returns `true` if the current time exactly matches `startHour:startMinute`.

### Examples

#### Afternoon/Evening (12:00–20:00)
```json
{
  "conditionType": "TimeOfDay",
  "parameters": {
    "startHour": "12",
    "startMinute": "0",
    "endHour": "20",
    "endMinute": "0"
  }
}
```

#### Night (22:00–06:00, wraps midnight)
```json
{
  "conditionType": "TimeOfDay",
  "parameters": {
    "startHour": "22",
    "endHour": "6"
  }
}
```

#### Specific Time (14:30)
```json
{
  "conditionType": "TimeOfDay",
  "parameters": {
    "startHour": "14",
    "startMinute": "30",
    "mode": "exact"
  }
}
```

---

## GameDay Condition

Gates content based on the current game day.

### Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `startDay` | int | Yes | — | Starting day number (1+) |
| `endDay` | int | No (if `mode="between"`) | — | Ending day number (1+) |
| `mode` | string | No | "onOrAfter" | Evaluation mode: `"between"`, `"exact"`, or `"onOrAfter"` |

### Modes

#### `onOrAfter` (default)
Returns `true` if the current day is `startDay` or any day after it.

#### `between`
Returns `true` if the current day is within the range `[startDay, endDay]` (inclusive on both ends).

#### `exact`
Returns `true` if the current day exactly matches `startDay`.


### Examples

#### Days 2–4
```json
{
  "conditionType": "GameDay",
  "parameters": {
    "startDay": "2",
    "endDay": "4"
  }
}
```

#### Day 5 Only
```json
{
  "conditionType": "GameDay",
  "parameters": {
    "startDay": "5",
    "mode": "exact"
  }
}
```

#### Day 3 or Later
```json
{
  "conditionType": "GameDay",
  "parameters": {
    "startDay": "3",
    "mode": "onOrAfter"
  }
}
```

---

## Combining Time and Day Conditions

To gate content that requires both a specific time AND a specific day, use multiple conditions:

```json
{
  "conditions": [
    {
      "conditionType": "TimeOfDay",
      "parameters": {
        "startHour": "14",
        "endHour": "18"
      }
    },
    {
      "conditionType": "GameDay",
      "parameters": {
        "startDay": "3",
        "mode": "onOrAfter"
      }
    }
  ]
}
```

This will only be true during afternoon hours (14:00–17:59) on day 3 or later.

