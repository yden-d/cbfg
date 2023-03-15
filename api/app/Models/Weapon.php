<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

/**
 * @property int $id
 * @property string $name
 * @property string $class
 */
class Weapon extends Model
{
	// Weapon classes
	public const CLASS_STRENGTH = 'STRENGTH';
	public const CLASS_DEXTERITY = 'DEXTERITY';
	public const CLASS_INTELLIGENCE = 'INTELLIGENCE';
	// Weapons
	public const WEAPON_SWORD = 'SWORD';
	public const WEAPON_SHIELD = 'SHIELD';
	public const WEAPON_BOW = 'BOW';
	public const WEAPON_WAND = 'WAND';
	public const WEAPON_STAFF = 'STAFF';

	/** Weapons array (arranged by class) */
	public const WEAPONS = [
		self::CLASS_STRENGTH => [
			self::WEAPON_SWORD,
			self::WEAPON_SHIELD,
		],
		self::CLASS_DEXTERITY => [
			self::WEAPON_BOW,
		],
		self::CLASS_INTELLIGENCE => [
			self::WEAPON_WAND,
			self::WEAPON_STAFF,
		],
	];

	protected $fillable = [
		'name',
		'class',
	];

	public static function getSword() {
		return self::where('class', self::CLASS_STRENGTH)
			->where('name', self::WEAPON_SWORD)
			->first();
	}

	public static function getShield() {
		return self::where('class', self::CLASS_STRENGTH)
			->where('name', self::WEAPON_SHIELD)
			->first();
	}

	public static function getBow() {
		return self::where('class', self::CLASS_DEXTERITY)
			->where('name', self::WEAPON_BOW)
			->first();
	}

	public static function getWand() {
		return self::where('class', self::CLASS_INTELLIGENCE)
			->where('name', self::WEAPON_WAND)
			->first();
	}

	public static function getStaff() {
		return self::where('class', self::CLASS_INTELLIGENCE)
			->where('name', self::WEAPON_STAFF)
			->first();
	}

}
