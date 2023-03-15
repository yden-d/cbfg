<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

/**
 * @property-read User $user
 * @property-read Weapon $weapon
 * @property-read Skill $neutral_light
 * @property-read Skill $neutral_heavy
 * @property-read Skill $side_light
 * @property-read Skill $side_heavy
 * @property-read Skill $up_light
 * @property-read Skill $up_heavy
 */
class Loadout extends Model
{
	protected $fillable = [
		'user_id',
		'weapon_id',
		'neutral_light',
		'neutral_heavy',
		'side_light',
		'side_heavy',
		'up_light',
		'up_heavy'
	];

	public function user(): BelongsTo
	{
		return $this->belongsTo(User::class);
	}

	public function weapon(): BelongsTo
	{
		return $this->belongsTo(Weapon::class);
	}

	public function neutralLight(): BelongsTo
	{
		return $this->belongsTo(Skill::class);
	}

	public function neutralHeavy(): BelongsTo
	{
		return $this->belongsTo(Skill::class);
	}

	public function sideLight(): BelongsTo
	{
		return $this->belongsTo(Skill::class);
	}

	public function sideHeavy(): BelongsTo
	{
		return $this->belongsTo(Skill::class);
	}

	public function upLight(): BelongsTo
	{
		return $this->belongsTo(Skill::class);
	}

	public function upHeavy(): BelongsTo
	{
		return $this->belongsTo(Skill::class);
	}
}
