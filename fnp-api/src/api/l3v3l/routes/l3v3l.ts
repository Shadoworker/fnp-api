/**
 * l3v3l router
 */

import { factories } from '@strapi/strapi';

export default {
  routes: [
    {
      method: 'GET',
      path: '/l3v3l/callback',
      handler: 'l3v3l.callback',
    },
    {
      method: 'GET',
      path: '/l3v3l/sample',
      handler: 'l3v3l.sample',
    }
  ]
}