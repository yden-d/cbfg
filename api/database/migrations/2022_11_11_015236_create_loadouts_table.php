<?php

use App\Models\Skill;
use App\Models\User;
use App\Models\Weapon;
use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration {
	/**
	 * Run the migrations.
	 *
	 * @return void
	 */
	public function up()
	{
		Schema::create('loadouts', function(Blueprint $table)
		{
			$table->id();
			$table->foreignIdFor(User::class);
			$table->foreignIdFor(Weapon::class);
			$table->foreignId('neutral_light_id')->nullable()->references('id')->on('skills');
			$table->foreignId('neutral_heavy_id')->nullable()->references('id')->on('skills');
			$table->foreignId('side_light_id')->nullable()->references('id')->on('skills');
			$table->foreignId('side_heavy_id')->nullable()->references('id')->on('skills');
			$table->foreignId('up_light_id')->nullable()->references('id')->on('skills');
			$table->foreignId('up_heavy_id')->nullable()->references('id')->on('skills');
			$table->timestamps();
		});

		Schema::table('users', function(Blueprint $table) {
			$table->foreignId('loadout_primary_id')->nullable()->references('id')->on('loadouts');
			$table->foreignId('loadout_secondary_id')->nullable()->references('id')->on('loadouts');
		});
	}

	/**
	 * Reverse the migrations.
	 *
	 * @return void
	 */
	public function down()
	{
		Schema::table('users', function(Blueprint $table) {
			$table->dropForeign('loadout_primary');
			$table->dropForeign('loadout_secondary');
		});
		Schema::dropIfExists('loadout');
	}
};
