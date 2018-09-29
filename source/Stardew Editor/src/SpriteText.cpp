#include "SpriteText.hpp"

#include <SFML/Graphics/RenderTarget.hpp>
#include <SFML/Graphics/Texture.hpp>

namespace 
{
    int getWidthOffsetForChar(char c)
    {
      if ((int) c <= 46U)
      {
        if ((int) c <= 36U)
        {
          if ((int) c != 33)
          {
            if ((int) c == 36)
              return 1;
            goto label_13;
          }
        }
        else
        {
          if ((int) c == 44 || (int) c == 46)
            return -2;
          goto label_13;
        }
      }
      else
      {
        if ((int) c <= 108U)
        {
          switch (c)
          {
            case '^':
              return -8;
            case 'i':
              break;
            case 'j':
            case 'l':
              goto label_9;
            default:
              goto label_13;
          }
        }
        else
        {
          switch (c)
          {
            case '¡':
              goto label_9;
            case 'ì':
            case 'í':
            case 'î':
            case 'ï':
              break;
            default:
              goto label_13;
          }
        }
        return -1;
      }
label_9:
      return -1;
label_13:
      return 0;
    }
}

SpriteText::SpriteText()
{
}

SpriteText::SpriteText( const sf::String& theStr, const sf::Texture& theFont )
:   str( theStr ),
    font( &theFont )
{
}

void SpriteText::setString( const sf::String& theStr )
{
    str = theStr;
    dirty = true;
}

void SpriteText::setFont( const sf::Texture& theFont )
{
    font = &theFont;
    dirty = true;
}

void SpriteText::setColor( const sf::Color& theColor )
{
    color = theColor;
    dirty = true;
}

const sf::String& SpriteText::getString() const
{
    return str;
}

const sf::Texture* SpriteText::getFont() const
{
    return font;
}

const sf::Color& SpriteText::getColor() const
{
    return color;
}

sf::FloatRect SpriteText::getLocalBounds() const
{
    build();
    return bounds;
}

sf::FloatRect SpriteText::getGlobalBounds() const
{
    return getTransform().transformRect( getLocalBounds() );
}

void SpriteText::draw( sf::RenderTarget& target, sf::RenderStates states ) const
{
    if ( dirty )
        build();
    
    states.transform *= getTransform();
    states.texture = font;
    target.draw( &vertices[ 0 ], vertices.size(), sf::PrimitiveType::Quads, states );
}

void SpriteText::build() const
{
    if ( font == nullptr || !dirty )
        return;
    
    vertices.clear();
    bounds = sf::FloatRect( 0, 0, 0, 0);
    
    sf::Vector2f pos( 0, 0 );
    bounds.left = pos.x;
    bounds.top = pos.y;
    
    std::string str = this->str.toAnsiString();
    for ( std::size_t i = 0; i < str.length(); ++i )
    {
        char c = str[ i ];
        if ( c == '\r' )
            continue;
        
        std::size_t space = str.find( ' ', i + 1 );
        
        if ( c == '\n' || c == '^' )
        {
            bounds.width = std::max( bounds.width, pos.x );
            pos.x = 0;
            pos.y += 18;
        }
        else
        {
            if ( space != std::string::npos && ( space - i ) * 10 >= 99999 )
            {
                bounds.width = std::max( bounds.width, pos.x );
                pos.x = 0;
                pos.y += 18;
            }
            
            int num = c - 32;
            sf::IntRect glyph( num * 8 % font->getSize().x, num * 8 / font->getSize().x * 16, 8, 16 );
            
            sf::Vector2f offset( 0, -1 );
            if ( std::isupper( c ) )
                offset.y -= 3;
            
            vertices.push_back( sf::Vertex( pos + offset + sf::Vector2f( 0          , 0            ), color, sf::Vector2f( glyph.left              , glyph.top                ) ) );
            vertices.push_back( sf::Vertex( pos + offset + sf::Vector2f( glyph.width, 0            ), color, sf::Vector2f( glyph.left + glyph.width, glyph.top                ) ) );
            vertices.push_back( sf::Vertex( pos + offset + sf::Vector2f( glyph.width, glyph.height ), color, sf::Vector2f( glyph.left + glyph.width, glyph.top + glyph.height ) ) );
            vertices.push_back( sf::Vertex( pos + offset + sf::Vector2f( 0          , glyph.height ), color, sf::Vector2f( glyph.left              , glyph.top + glyph.height ) ) );
            
            pos.x += glyph.width + getWidthOffsetForChar( c );
        }
    }
    
    bounds.width = std::max( bounds.width, pos.x );
    bounds.height = pos.y;
    
    dirty = false;
}
