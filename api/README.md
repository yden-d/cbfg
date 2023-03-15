# Class-based fighter game API usage

This directory contains the application for running the API.

## Quick Reference

Group                 | Action                                                               | Method | Route
----------------------|----------------------------------------------------------------------|--------|-----------------------------------
[Auth](#auth)         | [Register](#register-a-user)                                         | POST   | `/register`
[Auth](#auth)         | [Login](#login-as-an-existing-user)                                  | POST   | `/login`
[Auth](#auth)         | [WhoAmI](#get-information-about-the-authenticated-user)              | GET    | `/whoami`
[Users](#users)       | [Index](#retrieve-an-array-of-all-users)                             | GET    | `/users`
[Skills](#skills)     | [Index](#retrieve-an-array-of-all-skills)                            | GET    | `/skills`
[Skills](#skills)     | [View](#view-a-specific-skill)                                       | GET    | `/skills/{skill-id}`
[Skills](#skills)     | [Unlock](#unlock-a-skill-for-the-authenticated-user)                 | POST   | `/skills/{skill-id}/unlock`
[Loadouts](#loadouts) | [View](#view-a-specific-loadout)                                     | GET    | `/loadouts/{loadout-id}`
[Loadouts](#loadouts) | [Set Primary](#set-the-authenticated-users-primary-loadout)          | POST   | `/loadouts/{loadout-id}/primary`
[Loadouts](#loadouts) | [Set Secondary](#set-the-authenticated-users-secondary-loadout)      | POST   | `/loadouts/{loadout-id}/secondary`
[Loadouts](#loadouts) | [Equip Skill](#equip-a-skill-to-a-loadout)                           | POST   | `/loadouts/{loadout-id}/equip`
[Loadouts](#loadouts) | [Swap](#swap-the-authenticated-users-primary-and-secondary-loadouts) | POST   | `/loadouts/swap`

## Auth

User objects retrieved from the WhoAmI route have the following format:

Key               | Type                 | Description
------------------|----------------------|------------------------------------------------
id                | integer              | Unique ID for the user
username          | string               | Username of the user
loadout_primary   | [Loadout](#loadouts) | Primary loadout set for the user
loadout_secondary | [Loadout](#loadouts) | Secondary loadout set for the user
skills            | array                | [Skills](#skills) this user has unlocked
loadouts          | array                | Loadouts this user has set

### Register a user

POST `/register`

Request body:

Key      | Type   | Description
---------|--------|-----------------------------------
email    | string | Email to register for this user
username | string | Username to register for this user
password | string | Password to register for this user

Example:

```bash
curl -X POST "http://coms-402.merenze.com/register" -H "Content-Type: application/json" \
  -d '{"email":"alice@example","username":"alice","password:"alicepw"}'
```

### Login as an existing user

POST `/login`

Request body:

Key      | Type   | Description
---------|--------|---------------------------------
email    | string | Email of the registered user
password | string | Password for the registered user

Example:

```bash
curl -X POST "http://coms-402.merenze.com/register" -H "Content-Type: application/json" \
  -d '{"email":"alice@example","password":"alicepw"}'
```

### Get information about the authenticated user

GET `/whoami`

Response Body: [User](#auth) object.

Example:

```bash
curl -X GET "http://coms-402.merenze.com/whoami"
```

## Users

User objects retrieved from any route other than the [WhoAmI](#get-information-about-the-authenticated-user) route have the following format.

Key      | Type    | Description
---------|---------|-----------------------
id       | integer | Unique ID for the user
username | string  | Username of the user

### Retrieve an array of all users

GET `/users`

Response body: Array of [user](#users) objects.

Example:

```bash
curl -X GET "http://coms-402.merenze.com/users"
```

## Skills

Skill objects have the following format:

Key           | Type    | Description
--------------|---------|---------------------------------
id            | integer | Unique ID of the skill
weapon_id     | integer | Unique ID of the weapon for the skill
parent_id     | integer | Unique ID of the skill's parent
name          | string  | Name of the skill (not unique)
binding       | string  | Controller binding for the skill

In the case of a root skill, `parent_id` will be set to `null`.

### Retrieve an array of all skills

GET `/skills`

Response body: Array of [skill](#skills) objects.

### View a specific skill

GET `/skills/{skill-id}`

Response body: [Skill](#skills) object.

URL params:

Key      | Type    | Description
---------|---------|---------------------
skill_id | integer | Unique ID of a skill

Example:
```bash
curl -X GET "http://coms-402.merenze.com/skills/1"
```

### Unlock a skill for the authenticated user

POST `/skills/{skill_id}/unlock`

URL params:

Key      | Type    | Description
---------|---------|---------------------
skill_id | integer | Unique ID of a skill

Example:
```bash
curl -X GET "http://coms-402.merenze.com/skills/14/unlock"
```

## Loadouts

Loadout objects have the following format:

Key              | Type    | Description
-----------------|---------|------------------------------------------------------
id               | integer | Unique ID of the loadout
user_id          | integer | ID of the user who owns the loadout
weapon_id        | integer | ID of the weapon for the loadout
neutral_light_id | integer | ID of the neutral-light skill equipped to the loadout
neutral_heavy_id | integer | ID of the neutral-heavy skill equipped to the loadout
side_light_id    | integer | ID of the side-light skill equipped to the loadout
side_heavy_id    | integer | ID of the side-heavy skill equipped to the loadout
up_light_id      | integer | ID of the up-light skill equipped to the loadout
up_heavy_id      | integer | ID of the up-heavy skill equipped to the loadout

If there is no skill equipped to a binding, the skill's ID will be set to `null`.

### View a specific loadout

GET `/loadouts/{loadout-id}`

URL params:

Key        | Type    | Description
-----------|---------|----------------
loadout-id | integer | ID of a loadout

Response body: [Loadout](#loadouts) object.

Example:
```bash
curl -X GET "http://coms-402.merenze.com/loadouts/1"
```

Only the owner of a loadout has permission to view it; users must be authenticated to send this request.

### Set the authenticated user's primary loadout

POST `/loadouts/{loadout_id}/primary`

URL params:

Key        | Type    | Description
-----------|---------|----------------
loadout_id | integer | ID of a loadout

Example:
```bash
curl -X POST "http://coms-402.merenze.com/loadouts/1/primary"
```

- Only the owner of a loadout may set it as their primary loadout; users must be authenticated to send this request.
- If `loadout_id` is the ID of the user's secondary loadout, primary and secondary loadouts will be swapped.

### Set the authenticated user's secondary loadout

POST `/loadouts/{loadout_id}/secondary`

URL params:

Key        | Type    | Description
-----------|---------|----------------
loadout_id | integer | ID of a loadout

Example:
```bash
curl -X POST "http://coms-402.merenze.com/loadouts/2/secondary"
```

- Only the owner of a loadout may set it as their secondary loadout; users must be authenticated to send this request.
- If `loadout_id` is the ID of the user's primary loadout, primary and secondary loadouts will be swap.

### Equip a skill to a user's loadout

POST `/loadouts/{loadout_id}/equip/{skill_id}`

URL params:

Key        | Type    | Description
-----------|---------|----------------
loadout_id | integer | ID of a loadout
skill_id   | integer | ID of a skill

Example:
```bash
curl -X POST "http://coms-402.merenze.com/loadouts/1/equip/4"
```

- Only the owner of a loadout may equip a skill to it; users must be authenticated to send this request.
- The skill must already be unlocked by the user.
- The skill will automatically be equipped to the correct controller binding, replacing whatever skill was previously equipped to that binding.

### Swap the authenticated user's primary and secondary loadouts

POST `/loadouts/swap`

Example:

```bash
curl -X POST "http://coms-402.merenze.com/loadouts/swap"
```