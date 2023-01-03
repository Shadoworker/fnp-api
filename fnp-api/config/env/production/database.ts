export default ({ env }) => ({
  connection: {
    client: 'mysql',
    connection: {
      host: env('DATABASE_HOST', 'localhost'),
      port: env.int('DATABASE_PORT', 3306),
      database: env('DATABASE_NAME', 'fnp-db'),
      user: env('DATABASE_USERNAME', 'fnp'),
      password: env('DATABASE_PASSWORD', 'KayV3l2019uu'),
      ssl: env.bool('DATABASE_SSL', false),
    },
  },
});
