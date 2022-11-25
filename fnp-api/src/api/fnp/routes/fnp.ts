/**
 * fnp router
 */

import { factories } from '@strapi/strapi';

export default {
  routes: [
    {
      method: 'GET',
      path: '/fnp',
      handler: 'fnp.index',
    }
  ]
}