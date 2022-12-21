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
      path: '/l3v3l/getGameResources',
      handler: 'l3v3l.getGameResources',
    },

    {
      method: 'GET',
      path: '/l3v3l/getFish',
      handler: 'l3v3l.getFish',
    },

    {
      method: 'POST',
      path: '/l3v3l/updatePlayerResource',
      handler: 'l3v3l.updatePlayerResource',
    },

  ]
}