<?php

namespace App\Exceptions;

use Illuminate\Auth\Access\AuthorizationException;
use Illuminate\Auth\AuthenticationException;
use Illuminate\Database\Eloquent\ModelNotFoundException;
use Illuminate\Foundation\Exceptions\Handler as ExceptionHandler;
use Symfony\Component\HttpKernel\Exception\MethodNotAllowedHttpException;
use Symfony\Component\HttpKernel\Exception\NotFoundHttpException;
use Throwable;

class Handler extends ExceptionHandler
{
	/**
	 * A list of exception types with their corresponding custom log levels.
	 *
	 * @var array<class-string<\Throwable>, \Psr\Log\LogLevel::*>
	 */
	protected $levels = [
		//
	];

	/**
	 * A list of the exception types that are not reported.
	 *
	 * @var array<int, class-string<\Throwable>>
	 */
	protected $dontReport = [
		//
	];

	/**
	 * A list of the inputs that are never flashed to the session on validation exceptions.
	 *
	 * @var array<int, string>
	 */
	protected $dontFlash = [
		'current_password',
		'password',
		'password_confirmation',
	];

	public function render($request, Throwable $e)
	{
		if ($e instanceof AuthenticationException)
		{
			return response()->json([
				'message' => 'User must be authenticated to fulfill the request.',
			], 401);
		}
		if ($e instanceof AuthorizationException)
		{
			return response()->json([
				'message' => 'Action not authorized for the authenticated user.',
			], 403);
		}
		if ($e instanceof ModelNotFoundException)
		{
			$model = explode( '\\', $e->getModel())[2];
			return response()->json([
				'message' => "No $model with id {$e->getIds()[0]}.",
			], 404);
		}
		if ($e instanceof NotFoundHttpException)
		{
			return response()->json([
				'message' => 'The specified resource could not be found.',
			], 404);
		}
		if ($e instanceof MethodNotAllowedHttpException)
		{
			return response()->json([
				'message' => sprintf("%s method not supported for this route.", $request->method()),
			], 405);
		}

		// Unhandled exception
		return response()->json(config('app.debug')
			? [
				'exception' => get_class($e),
				'message' => $e->getMessage(),
				'line' => $e->getLine(),
				'trace' => $e->getTrace(),
			]
			: [
				'message' => 'Server encountered an unexpected problem while handling this request.',
			], 500);
	}

	/**
	 * Register the exception handling callbacks for the application.
	 *
	 * @return void
	 */
	public function register()
	{
		$this->reportable(function(Throwable $e)
		{
			//
		});
	}
}
