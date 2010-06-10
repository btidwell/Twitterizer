﻿//-----------------------------------------------------------------------
// <copyright file="TwitterEntityCollection.cs" company="Patrick 'Ricky' Smith">
//  This file is part of the Twitterizer library (http://www.twitterizer.net)
// 
//  Copyright (c) 2010, Patrick "Ricky" Smith (ricky@digitally-born.com)
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without modification, are 
//  permitted provided that the following conditions are met:
// 
//  - Redistributions of source code must retain the above copyright notice, this list 
//    of conditions and the following disclaimer.
//  - Redistributions in binary form must reproduce the above copyright notice, this list 
//    of conditions and the following disclaimer in the documentation and/or other 
//    materials provided with the distribution.
//  - Neither the name of the Twitterizer nor the names of its contributors may be 
//    used to endorse or promote products derived from this software without specific 
//    prior written permission.
// 
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
//  IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
//  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
//  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//  WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
//  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
//  POSSIBILITY OF SUCH DAMAGE.
// </copyright>
// <author>Ricky Smith</author>
// <summary>The twitter entity collection class</summary>
//-----------------------------------------------------------------------

namespace Twitterizer.Entities
{
    using System;
    using System.Collections.ObjectModel;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents multiple <see cref="Twitterizer.Entities.TwitterEntity"/> objects.
    /// </summary>
    [Serializable]
    public class TwitterEntityCollection : Collection<TwitterEntity>
    {
        /// <summary>
        /// The Json converter for <see cref="TwitterEntityCollection"/> data.
        /// </summary>
        internal class Converter : JsonConverter
        {
            /// <summary>
            /// Determines whether this instance can convert the specified object type.
            /// </summary>
            /// <param name="objectType">Type of the object.</param>
            /// <returns>
            /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
            /// </returns>
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(TwitterEntityCollection);
            }

            /// <summary>
            /// Reads the JSON representation of the object.
            /// </summary>
            /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param>
            /// <param name="objectType">Type of the object.</param>
            /// <param name="existingValue">The existing value of object being read.</param>
            /// <param name="serializer">The calling serializer.</param>
            /// <returns>The object value.</returns>
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                TwitterEntityCollection result = existingValue as TwitterEntityCollection;
                if (result == null)
                    result = new TwitterEntityCollection();

                int startDepth = reader.Depth;
                string entityType = string.Empty;
                TwitterEntity entity = null;

                while (reader.Read() && reader.Depth >= startDepth)
                {
                    if (reader.TokenType == JsonToken.PropertyName && reader.Depth == startDepth + 1)
                    {
                        entityType = (string)reader.Value;
                        continue;
                    }

                    switch (entityType)
                    {
                        case "urls":
                            if (reader.TokenType == JsonToken.StartObject)
                            entity = new TwitterUrlEntity();

                            if (reader.TokenType == JsonToken.PropertyName)
                            {
                                if ((string)reader.Value == "url")
                                {
                                    reader.Read();
                                    ((TwitterUrlEntity)entity).Url = (string)reader.Value;
                                }
                            }

                            break;
                        case "user_mentions":
                            if (reader.TokenType == JsonToken.StartObject)
                            entity = new TwitterMentionEntity();

                            if (reader.TokenType == JsonToken.PropertyName)
                            {
                                if ((string)reader.Value == "screen_name")
                                {
                                    reader.Read();
                                    ((TwitterMentionEntity)entity).ScreenName = (string)reader.Value;
                                }

                                if ((string)reader.Value == "name")
                                {
                                    reader.Read();
                                    ((TwitterMentionEntity)entity).Name = (string)reader.Value;
                                }

                                if ((string)reader.Value == "id")
                                {
                                    reader.Read();
                                    ((TwitterMentionEntity)entity).UserId = Convert.ToDecimal(reader.Value);
                                }
                            }

                            break;
                        case "hashtags":
                            if (reader.TokenType == JsonToken.StartObject)
                            entity = new TwitterHashTagEntity();

                            if (reader.TokenType == JsonToken.PropertyName)
                            {
                                if ((string)reader.Value == "text")
                                {
                                    reader.Read();
                                    ((TwitterHashTagEntity)entity).Text = (string)reader.Value;
                                }
                            }

                            break;
                        default:
                            break;
                    }
                    
                    // Read the indicies (for all entities)
                    if (reader.TokenType == JsonToken.PropertyName && (string)reader.Value == "indices")
                    {
                        reader.Read();
                        reader.Read();
                        entity.StartIndex = (long)reader.Value;
                        reader.Read();
                        entity.EndIndex = (long)reader.Value;
                    }

                    if (reader.TokenType == JsonToken.EndObject && entity != null)
                    {
                        result.Add(entity);
                        entity = null;
                    }
                }

                return result;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}