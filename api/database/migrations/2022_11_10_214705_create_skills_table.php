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
		// Destroy old tables
		Schema::dropIfExists('user_skill_relations');
		Schema::dropIfExists('skills');

		Schema::create('skills', function(Blueprint $table)
		{
			$table->id();
			$table->foreignIdFor(Weapon::class);
			$table->foreignId('parent_id')->nullable()->references('id')->on('skills');
			$table->string('name');
			$table->string('binding');
			$table->timestamps();
		});

		Schema::create('skill_user', function(Blueprint $table) {
			$table->foreignIdFor(Skill::class);
			$table->foreignIdFor(User::class);
			$table->primary(['skill_id', 'user_id']);
		});
	}

	/**
	 * Reverse the migrations.
	 *
	 * @return void
	 */
	public function down()
	{
		Schema::dropIfExists('skill_user');
		Schema::table('skills', function (Blueprint $table) {
			$table->dropColumn('parent');
		});
		Schema::dropIfExists('skills');
	}
};
