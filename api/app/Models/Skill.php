<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Collection;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Illuminate\Database\Eloquent\Relations\HasMany;

/**
 * @property int $id
 * @property-read Weapon $weapon The weapon associated with this skill.
 * @property-read Skill $parent The skill upon which this skill is directly dependent.
 * @property Collection $children The skills directly dependent upon this skill.
 * @property string $name
 * @property string $binding The controller binding for this skill.
 */
class Skill extends Model
{
	// Controller bindings for skills
	public const BINDING_NEUTRAL_LIGHT = 'NEUTRAL LIGHT';
	public const BINDING_NEUTRAL_HEAVY = 'NEUTRAL HEAVY';
	public const BINDING_SIDE_LIGHT = 'SIDE LIGHT';
	public const BINDING_SIDE_HEAVY = 'SIDE HEAVY';
	public const BINDING_UP_LIGHT = 'UP LIGHT';
	public const BINDING_UP_HEAVY = 'UP HEAVY';

	protected $fillable = [
		'weapon_id',
		'parent',
		'name',
		'binding',
	];

	public function parent(): BelongsTo
	{
		return $this->belongsTo(Skill::class);
	}

	public function children(): HasMany
	{
		return $this->hasMany(Skill::class);
	}

	public function weapon(): BelongsTo
	{
		return $this->belongsTo(Weapon::class);
	}

}
