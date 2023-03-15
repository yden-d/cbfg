<?php

namespace App\Models;

// use Illuminate\Contracts\Auth\MustVerifyEmail;
use Illuminate\Database\Eloquent\Collection;
use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Illuminate\Database\Eloquent\Relations\BelongsToMany;
use Illuminate\Database\Eloquent\Relations\HasMany;
use Illuminate\Database\Eloquent\Relations\HasOne;
use Illuminate\Foundation\Auth\User as Authenticatable;
use Illuminate\Notifications\Notifiable;
use Laravel\Sanctum\HasApiTokens;

/**
 * @property string $username
 * @property string $email
 * @property string $password
 * @property-read Collection $skills
 * @property-read Collection $loadouts
 * @property-read Loadout $loadoutPrimary
 * @property-read Loadout $loadoutSecondary
 */
class User extends Authenticatable
{
	use HasApiTokens, HasFactory, Notifiable;

	/**
	 * The attributes that are mass assignable.
	 *
	 * @var array<int, string>
	 */
	protected $fillable = [
		'username',
		'email',
		'password',
	];

	/**
	 * The attributes that should be hidden for serialization.
	 *
	 * @var array<int, string>
	 */
	protected $hidden = [
		'email',
		'password',
		'remember_token',
	];

	/**
	 * The attributes that should be cast.
	 *
	 * @var array<string, string>
	 */
	protected $casts = [
		'email_verified_at' => 'datetime',
	];

	public function skills(): BelongsToMany
	{
		return $this->belongsToMany(Skill::class);
	}

	public function loadouts(): HasMany
	{
		return $this->hasMany(Loadout::class);
	}

	public function loadoutPrimary(): BelongsTo
	{
		return $this->belongsTo(Loadout::class);
	}

	public function loadoutSecondary(): BelongsTo
	{
		return $this->belongsTo(Loadout::class);
	}

	public function hasSkill(Skill $skill): bool
	{
		foreach ($this->skills as $s)
		{
			if ($s->id == $skill->id)
			{
				return true;
			}
		}
		return false;
	}

	public function canUnlock(Skill $skill): bool
	{
		// TODO other conditions
		return is_null($skill->parent) || $this->hasSkill($skill->parent);
	}

	public function addSkill(Skill $skill)
	{
		$this->skills()->attach($skill->id);
	}

	public function removeSkill(Skill $skill)
	{
		$this->skills()->detach($skill->id);
	}
}
