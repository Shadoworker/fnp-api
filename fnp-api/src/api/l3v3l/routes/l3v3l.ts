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
      path: '/l3v3l/who',
      handler: 'l3v3l.getUserData',
    },

    {
      method: 'GET',
      path: '/l3v3l/games',
      handler: 'l3v3l.getGames',
    },
    
    {
      method: 'GET',
      path: '/l3v3l/get-a-fish',
      handler: 'l3v3l.getAFish',
    }
  ]
}